/*
 * AudioData.cs
 * AudioClipデータの格納場所を作成するためのスクリプト
 */
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class NamedClip
    {
        public string name;     // 識別用の名前
        public AudioClip clip;  // 実際の音源
        public bool loop;       // ループフラグ
        [Range(0f, 1f)] public float volume = 1f;   // 音量
    }

    [Tooltip("AudioClip管理・格納用配列")]
    public NamedClip[] bgmClips;    // BGM
    public NamedClip[] seClips;     // SE
    public NamedClip[] envClips;    // 環境音
}
