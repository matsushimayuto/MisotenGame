/*
 *AudioManager.cs
 *AudioDataの再生と管理・音源に対してのフェード処理を管理するスクリプト
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioMixer")] 
    public AudioMixer mixer;
    public AudioMixerGroup bgmGroupA;
    public AudioMixerGroup bgmGroupB;
    public AudioMixerGroup seGroup;
    public AudioMixerGroup envGroup;

    [Header("Audiodataを参照用の格納用変数")]
    public AudioData audioData;

    [Header("SE Pool")]
    [SerializeField] private int sePoolSize = 8;

    [Tooltip("参照を楽に行うための辞書変数")]
    private Dictionary<string, AudioSetting> bgmDict = new();
    private Dictionary<string, AudioSetting> seDict = new();
    private Dictionary<string, AudioSetting> envDict = new();
 
    [Tooltip("クロスフェード用変数")]
    private AudioSource bgmA;
    private AudioSource bgmB;
    private bool isUsingA = true;

    [Tooltip("環境音とSE再生用プールの変数")]
    private AudioSource envSource;
    private Queue<AudioSource> sePool = new();

    private const string BGM_A_PARAM = "BGM_A";
    private const string BGM_B_PARAM = "BGM_B";
    private const string SE_PARAM  = "SE_Volume";
    private const string ENV_PARAM = "ENV_Volume";

    //ーーーーーーーーーーーーーーーーーーーーーーーーー
    // 以下に関数記述

    // SingletonやAudioMixerなどの初期化を行う関数
    private void Awake()
    {
        // AudioManagerの初期化
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(audioData == null)
        {
            Debug.LogError("AudioData is null");
            enabled = false;
            return;
        }

        InitSources();
        BuildDictionaries();

    }

    private void InitSources()
    {
        bgmA = CreateSource(bgmGroupA, true);
        bgmB = CreateSource(bgmGroupB, true);
        envSource = CreateSource(envGroup, true);

        for (int i = 0; i < sePoolSize; i++)
            sePool.Enqueue(CreateSource(seGroup, false));
    }

    private AudioSource CreateSource(AudioMixerGroup group, bool loop)
    {
        var s = gameObject.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.loop = loop;
        s.playOnAwake = false;
        s.volume = 1.0f;
        return s;
    }

    private void BuildDictionaries()
    {
        foreach (var a in audioData.bgmClips) bgmDict[a.name] = a;
        foreach (var a in audioData.seClips) seDict[a.name] = a;
        foreach (var a in audioData.envClips) envDict[a.name] = a;
    }

    //====================================
    //  BGM
    //====================================

    // PlayBGM : 引数:string(再生するBGMの名前), float(フェードにかける時間), 戻り値:なし 
    // BGMの再生とBGMをクロスフェードして再生するための関数
    // 使用例：AudioManager.Instance.PlayBGM("StageBGM", 1.0f);
    // 上記の書き方でBGMを再生できる、クロスフェードさせたい場合も同様に使用する
    public void PlayBGM(string name, float fadeTime = 1f)
    {
        // 辞書内に変数があるかどうか
        if(!bgmDict.TryGetValue(name, out var setting)) return;


        // 現在再生している音源と、次に再生する音源を識別
        AudioSource active = isUsingA ? bgmA : bgmB;
        AudioSource next   = isUsingA ? bgmB : bgmA;

        // 同一BGMガード
        if (active.clip == setting.clip) return;

        // 次に再生する音源の設定を更新
        next.clip = setting.clip;
        next.loop = setting.loop;
        next.Play();

        // クロスフェードコルーチン開始
        StopAllCoroutines();
        StartCoroutine(CrossFadeMixer(active, next, fadeTime, setting.volume));
        // 使用中フラグを下げる
        isUsingA = !isUsingA;
    }

    // StopBGM : 引数:float(フェードにかける時間)、戻り値:なし
    // BGMをフェードアウトさせて停止させる関数
    public void StopBGM(float fadeTime = 1f)
    {
        StopAllCoroutines();
 
        StartCoroutine(FadeOutAndStopBGM(fadeTime));
    }

    //====================================
    //  SE
    //====================================

    // PlaySE : 引数:string(再生するSEの名称)、戻り値:なし
    // 使用例 : AudioManager.Instance.PlaySE("足音");
    public void PlaySE(string name)
    {
        if (!seDict.TryGetValue(name, out var setting)) return;

        if (sePool.Count == 0) return;

        var src = sePool.Dequeue();
        src.clip = setting.clip;
        src.PlayOneShot(setting.clip, setting.volume);
        StartCoroutine(ReturnSE(src));
    }

    private IEnumerator ReturnSE(AudioSource src)
    {
        yield return new WaitUntil(() => !src.isPlaying);
        sePool.Enqueue(src);
    }

    //====================================
    //  ENV
    //====================================

    // PlayEnv : 引数:string(再生する環境音の名称)
    // 環境音を再生するための関数
    public void PlayEnv(string name)
    {
        if (!envDict.TryGetValue(name, out var setting)) return;

        envSource.clip = setting.clip;
        envSource.loop = setting.loop;
        envSource.Play();

        mixer.SetFloat(ENV_PARAM, LinearToDb(setting.volume));
    }


    // StopEnv : 引数:float(フェードにかける時間)、戻り値:なし
    public void StopEnv(float fadeTime = 1f)
    {
        StopCoroutine(nameof(FadeOutAndStopEnv));
        StartCoroutine(FadeOutAndStopEnv(fadeTime));
    }


    //====================================
    //  フェード
    //====================================

    // クロスフェード用コルーチン
    private IEnumerator CrossFadeMixer(
    AudioSource from,
    AudioSource to,
    float time,
    float targetVol)
    {
        string fromParam = GetBgmParam(from);
        string toParam = GetBgmParam(to);

        mixer.GetFloat(fromParam, out float fromStartDb);

        float toTargetDb = LinearToDb(targetVol);

        // 初期化
        mixer.SetFloat(toParam, -80f);

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float r = t / time;

            mixer.SetFloat(fromParam, Mathf.Lerp(fromStartDb, -80f, r));
            mixer.SetFloat(toParam, Mathf.Lerp(-80f, toTargetDb, r));

            yield return null;
        }

        mixer.SetFloat(fromParam, -80f);
        mixer.SetFloat(toParam, toTargetDb);

        from.Stop();
    }

    //=================================
    // フェードアウト用コルーチン
    //=================================

    // BGM用フェードアウトコルーチン
    private IEnumerator FadeOutAndStopBGM(float time)
    {
        AudioSource a = bgmA;
        AudioSource b = bgmB;

        bool aPlaying = a.isPlaying;
        bool bPlaying = b.isPlaying;

        if (!aPlaying && !bPlaying) yield break;

        float t = 0f;

        // 開始音量取得
        mixer.GetFloat(BGM_A_PARAM, out float aStart);
        mixer.GetFloat(BGM_B_PARAM, out float bStart);

        while (t < time)
        {
            t += Time.deltaTime;
            float r = t / time;

            if (aPlaying)
                mixer.SetFloat(BGM_A_PARAM, Mathf.Lerp(aStart, -80f, r));

            if (bPlaying)
                mixer.SetFloat(BGM_B_PARAM, Mathf.Lerp(bStart, -80f, r));

            yield return null;
        }

        if (aPlaying)
        {
            mixer.SetFloat(BGM_A_PARAM, -80f);
            a.Stop();
        }

        if (bPlaying)
        {
            mixer.SetFloat(BGM_B_PARAM, -80f);
            b.Stop();
        }
    }

    // ENV用フェードアウトコルーチン
    private IEnumerator FadeOutAndStopEnv(float time)
    {
        if (!envSource.isPlaying)
            yield break;

        mixer.GetFloat(ENV_PARAM, out float startDb);

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            mixer.SetFloat(ENV_PARAM, Mathf.Lerp(startDb, -80f, t / time));
            yield return null;
        }

        mixer.SetFloat(ENV_PARAM, -80f);
        envSource.Stop();

        // 次回再生用に初期化（重要）
        mixer.SetFloat(ENV_PARAM, startDb);
    }


    private float LinearToDb(float v)
    {
        return Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
    }

    private string GetBgmParam(AudioSource src)
    {
        return src == bgmA ? BGM_A_PARAM : BGM_B_PARAM;
    }

}
