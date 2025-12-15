using UnityEngine;

public class StageAreaTrigger : MonoBehaviour
{
    public int worldNumber;
    public int stageNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<StageEnterContoroller>()
            ?.SetStageCandidate(this);

        Debug.Log($"Stage Area: {worldNumber}-{stageNumber}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<StageEnterContoroller>()
            ?.ClearStageCandidate(this);
    }


}
