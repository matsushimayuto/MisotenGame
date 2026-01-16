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

    [Header("AudioData")]
    public AudioData audioData;

    [Header("SE Pool")]
    [SerializeField] private int sePoolSize = 8;

    private Dictionary<string, AudioSetting> bgmDict = new();
    private Dictionary<string, AudioSetting> seDict = new();
    private Dictionary<string, AudioSetting> envDict = new();

    private AudioSource bgmA;
    private AudioSource bgmB;
    private AudioSource envSource;
    private Queue<AudioSource> sePool = new();

    private AudioSource currentBgm;
    private AudioSource nextBgm;

    private Coroutine bgmFadeCoroutine;
    private Coroutine envFadeCoroutine;

    private const string BGM_A_PARAM = "BGM_A";
    private const string BGM_B_PARAM = "BGM_B";
    private const string ENV_PARAM 　= "ENV_Volume";

    private float currentBgmBaseVolume;
    private float nextBgmBaseVolume;

    //ーーーーーーーーーーーーーーーーーーーーーーーーー
    // 以下に関数記述


    //============================
    // 初期化
    //============================
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioData == null)
        {
            Debug.LogError("[AudioManager] AudioData is null");
            enabled = false;
            return;
        }

        InitSources();
        BuildDictionaries();
        ValidateMixerParams();
    }

    private void InitSources()
    {
        bgmA = CreateSource(bgmGroupA, true);
        bgmB = CreateSource(bgmGroupB, true);
        envSource = CreateSource(envGroup, true);

        currentBgm = bgmA;
        nextBgm = bgmB;

        for (int i = 0; i < sePoolSize; i++)
            sePool.Enqueue(CreateSource(seGroup, false));
    }

    private AudioSource CreateSource(AudioMixerGroup group, bool loop)
    {
        var s = gameObject.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.loop = loop;
        s.playOnAwake = false;
        return s;
    }

    private void BuildDictionaries()
    {
        foreach (var a in audioData.bgmClips) bgmDict[a.name] = a;
        foreach (var a in audioData.seClips) seDict[a.name] 　= a;
        foreach (var a in audioData.envClips) envDict[a.name] = a;
    }

    private void ValidateMixerParams()
    {
        CheckParam(BGM_A_PARAM);
        CheckParam(BGM_B_PARAM);
        CheckParam(ENV_PARAM);
    }

    private void CheckParam(string param)
    {
        if (!mixer.GetFloat(param, out _))
            Debug.LogError($"[AudioManager] Missing Mixer Param: {param}");
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
        if (!bgmDict.TryGetValue(name, out var setting))
        {
            Debug.LogWarning($"[AudioManager] BGM not found: {name}");
            return;
        }

        // 初回再生（クロスフェードしない）
        if (!currentBgm.isPlaying)
        {
            currentBgm.clip = setting.clip;
            currentBgm.loop = setting.loop;
            currentBgmBaseVolume = setting.volume;

            currentBgm.volume = currentBgmBaseVolume;
            currentBgm.Play();
            return;
        }

        // 次BGM準備
        nextBgm.clip = setting.clip;
        nextBgm.loop = setting.loop;
        nextBgmBaseVolume = setting.volume;

        nextBgm.volume = nextBgmBaseVolume;
        nextBgm.Play();

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(
            CrossFadeBGM(fadeTime)
        );
    }


    // StopBGM : 引数:float(フェードにかける時間)、戻り値:なし
    // BGMをフェードアウトさせて停止させる関数
    public void StopBGM(float fadeTime = 1f)
    {
        if (!currentBgm.isPlaying)
            return;

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutCurrentBGM(fadeTime));

    }

    //====================================
    //  SE
    //====================================

    // PlaySE : 引数:string(再生するSEの名称)、戻り値:なし
    // 使用例 : AudioManager.Instance.PlaySE("足音");
    public void PlaySE(string name)
    {
        if (!seDict.TryGetValue(name, out var setting))
            return;

        if (sePool.Count == 0)
            return; // 再生しない

        var src = sePool.Dequeue();
        src.PlayOneShot(setting.clip, setting.volume);
        StartCoroutine(ReturnSE(src));
    }


    private IEnumerator ReturnSE(AudioSource src)
    {
        yield return new WaitUntil(() => !src.isPlaying);
        if (!sePool.Contains(src))
            sePool.Enqueue(src);
    }

    //====================================
    //  ENV
    //====================================

    // PlayEnv : 引数:string(再生する環境音の名称)
    // 環境音を再生するための関数
    public void PlayEnv(string name)
    {
        if (!envDict.TryGetValue(name, out var setting))
        {
            Debug.LogWarning($"[AudioManager] ENV not found: {name}");
            return;
        }

        envSource.clip = setting.clip;
        envSource.loop = setting.loop;
        envSource.Play();

        mixer.SetFloat(ENV_PARAM, LinearToDb(setting.volume));
    }



    // StopEnv : 引数:float(フェードにかける時間)、戻り値:なし
    public void StopEnv(float fadeTime = 1f)
    {
        if (envFadeCoroutine != null)
            StopCoroutine(envFadeCoroutine);

        envFadeCoroutine = StartCoroutine(FadeOutEnv(fadeTime));
    }


    //====================================
    //  フェード
    //====================================

    // クロスフェード用コルーチン
    private IEnumerator CrossFadeBGM(float time)
    {
        float fromBase = currentBgmBaseVolume;
        float toBase = nextBgmBaseVolume;

        currentBgm.volume = fromBase;
        nextBgm.volume = 0f;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float r = Mathf.SmoothStep(0f, 1f, t / time);

            currentBgm.volume = Mathf.Lerp(fromBase, 0f, r);
            nextBgm.volume = Mathf.Lerp(0f, toBase, r);
            yield return null;
        }

        currentBgm.volume = 0f;
        currentBgm.Stop();

        nextBgm.volume = toBase;

        // ★ 状態更新（重要）
        currentBgmBaseVolume = nextBgmBaseVolume;
        (currentBgm, nextBgm) = (nextBgm, currentBgm);
    }

    //=================================
    // フェードアウト用コルーチン
    //=================================

    // BGM用フェードアウトコルーチン
    private IEnumerator FadeOutCurrentBGM(float time)
    {
        float start = currentBgm.volume;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            currentBgm.volume = Mathf.Lerp(start, 0f, t / time);
            yield return null;
        }

        currentBgm.Stop();
        currentBgm.volume = 0f;

        
        currentBgmBaseVolume = 1f;
        currentBgm.clip = null;
    }

    // ENV用フェードアウトコルーチン
    private IEnumerator FadeOutEnv(float time)
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
    }


    //============================
    // Utility
    //============================
    private float LinearToDb(float v)
    {
        return Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
    }

    private string GetBgmParam(AudioSource src)
    {
        return src == bgmA ? BGM_A_PARAM : BGM_B_PARAM;
    }

}
