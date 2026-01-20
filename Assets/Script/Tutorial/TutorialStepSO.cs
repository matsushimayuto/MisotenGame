using UnityEngine;

// チュートリアルを進めるための条件一覧
public enum TutorialTriggerType
{
    None,
    PressTrigger,
    MoveToTarget,
    HitTarget,
    TimeElapsed,
    CustomEvent
}

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Scriptable Objects/TutorialStep")]
public class TutorialStepSO : ScriptableObject
{
    [TextArea(2, 4)]
    public string message;     // 表示メッセージ

    public Sprite image;       // 表示画像

    public Sprite textimage;   // 表示テキスト画像

    public TutorialTriggerType trigger = TutorialTriggerType.None;  // 条件

    public float time = 1f; // TimeElapsed用

}


