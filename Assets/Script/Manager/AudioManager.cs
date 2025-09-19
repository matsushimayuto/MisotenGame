/*
 *AudioManager.cs
 *AudioDataの再生と管理・音源に対してのフェード処理を管理するスクリプト
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioMixerの設定")] 
    public AudioMixer mixer;
    public AudioMixerGroup bgmGroup;
    public AudioMixerGroup seGroup;
    public AudioMixerGroup envGroup;

    [Header("Audiodataを参照するための格納用変数")]
    public AudioData audioData;

    [Tooltip("参照を楽に行うための辞書変数")]
    private Dictionary<string, AudioClip> bgmDict = new();
    private Dictionary<string, AudioClip> seDict = new();
    private Dictionary<string, AudioClip> envDict = new();
    private Dictionary<string, bool>      loopDict = new();
    private Dictionary<string, float>     volumeDict = new();
 
    [Tooltip("クロスフェード用変数")]
    private AudioSource bgmSourceA;
    private AudioSource bgmSourceB;
    private bool isUsingA = true;

    [Tooltip("環境音とSE再生用の変数")]
    private AudioSource seSource;
    private AudioSource envSource;


    //ーーーーーーーーーーーーーーーーーーーーーーーーー
    // 以下に関数記述

    // SingletonやAudioMixerなどの初期化を行う関数
    private void Awake()
    {
        // AudioManagerの初期化
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // 再生用音源の初期化
        bgmSourceA = gameObject.AddComponent<AudioSource>();
        bgmSourceB = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();
        envSource = gameObject.AddComponent<AudioSource>();

        // AudioMixerGroupの初期化
        bgmSourceA.outputAudioMixerGroup = bgmGroup;
        bgmSourceB.outputAudioMixerGroup = bgmGroup;
        seSource.outputAudioMixerGroup   = seGroup;
        envSource.outputAudioMixerGroup  = envGroup;

        // ScriptableObjectから辞書化
        foreach (var n in audioData.bgmClips) { bgmDict[n.name] = n.clip; loopDict[n.name] = n.loop; volumeDict[n.name] = n.volume; }
        foreach (var n in audioData.seClips)  { seDict[n.name]  = n.clip; loopDict[n.name] = n.loop; volumeDict[n.name] = n.volume; }
        foreach (var n in audioData.envClips) { envDict[n.name] = n.clip; loopDict[n.name] = n.loop; volumeDict[n.name] = n.volume; }


        // ------最初に開始したい状態を選んで書いてください------
        GameManager.Instance.ChangeState(GameState.Title);
    }

    // PlayBGM : 引数:string(再生するBGMの名前), float(フェードにかける時間), 戻り値:なし 
    // BGMの再生とBGMをクロスフェードして再生するための関数
    // 使用例：AudioManager.Instance.PlayBGM("StageBGM", 1.0f);
    // 上記の書き方でBGMを再生できる、クロスフェードさせたい場合も同様に使用する
    public void PlayBGM(string name, float fadeTime = 1f)
    {
        // 辞書内に変数があるかどうか
        if (!bgmDict.ContainsKey(name)) return;

        // 現在再生している音源と、次に再生する音源を識別
        AudioSource active = isUsingA ? bgmSourceA : bgmSourceB;
        AudioSource next   = isUsingA ? bgmSourceB : bgmSourceA;

        // 次に再生する音源の設定を更新
        next.clip   = bgmDict[name];
        next.loop   = loopDict[name];
        next.volume = 0;
        next.Play();

        // クロスフェードコルーチン開始
        StartCoroutine(CrossFade(active, next, fadeTime, volumeDict[name]));
        isUsingA = !isUsingA;
    }

    // StopBGM : 引数:float(フェードにかける時間)、戻り値:なし
    // BGMをフェードアウトさせて停止させる関数
    public void StopBGM(float fadeTime = 1f)
    {
        AudioSource active = isUsingA? bgmSourceA : bgmSourceB;
        StartCoroutine(FadeOut(active, fadeTime));
    }

    // PlaySE : 引数:string(再生するSEの名称)、戻り値:なし
    // 使用例 : AudioManager.Instance.PlaySE("足音");
    public void PlaySE(string name)
    {
        if (!seDict.ContainsKey(name)) return;
        seSource.PlayOneShot(seDict[name], volumeDict[name]);
    }

    // PlayEnv : 引数:string(再生する環境音の名称)
    // 環境音を再生するための関数
    public void PlayEnv(string name)
    {
        if (!envDict.ContainsKey(name)) return;
        envSource.clip   = envDict[name];
        envSource.loop   = loopDict[name];
        envSource.volume = volumeDict[name];
        envSource.Play();
    }

    // StopEnv : 引数:float(フェードにかける時間)、戻り値:なし
    public void StopEnv(float fadeTime = 1f)
    {
        StartCoroutine(FadeOut(envSource, fadeTime));
    }


    //ーーーーーーーーーーーーーフェード関係コルーチンーーーーーーーーーーーーー

    // クロスフェード用コルーチン
    private IEnumerator CrossFade(AudioSource from, AudioSource to, float time, float targetVal)
    {
        // フェード用の変数を初期化
        float t = 0.0f;
        to.volume = 0f;

        // クロスフェード処理
        while (t < time)
        {
            // フェードを行う速度を算出
            t += Time.deltaTime;
            float rate  = t / time;
            from.volume = Mathf.Lerp(targetVal, 0, rate);
            to.volume   = Mathf.Lerp(0, targetVal, rate);
            yield return null;
        }

        from.Stop();
        to.volume = targetVal;
    }

    // フェードアウト用コルーチン
    private IEnumerator FadeOut(AudioSource source, float time)
    {
        if (!source.isPlaying) yield break;

        float startVol = source.volume;
        float t = 0f;

        while(t < startVol)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / time);
            yield return null;
        }
        source.Stop();
        source.volume = startVol;   // 元に戻しておく
    }

}
