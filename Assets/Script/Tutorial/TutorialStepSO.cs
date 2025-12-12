using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Scriptable Objects/TutorialStep")]
public class TutorialStepSO : ScriptableObject
{
    public enum TutorialTriggerType
    {
        None,
        PressTrigger,
        MoveTrigger,
    }



}
