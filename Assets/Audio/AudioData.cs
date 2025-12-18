/*
 * AudioData.cs
 * AudioClipデータの格納場所を作成するためのスクリプト
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public List<AudioSetting> bgmClips;
    public List<AudioSetting> seClips;
    public List<AudioSetting> envClips;
}

[System.Serializable]
public class AudioSetting
{
    public string name;     // 識別用の名前
    public AudioClip clip;  // 実際の音源
    public bool loop;       // ループフラグ
    [Range(0f, 1f)] public float volume = 1f;   // 音量
}
