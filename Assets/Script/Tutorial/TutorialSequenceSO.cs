using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSequenceSO", menuName = "Scriptable Objects/TutorialSequenceSO")]
public class TutorialSequenceSO : ScriptableObject
{
    public TutorialStepSO[] steps;
}
