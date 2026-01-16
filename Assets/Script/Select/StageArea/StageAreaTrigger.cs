using DoorScript;
using UnityEngine;

public class StageAreaTrigger : MonoBehaviour
{
    public int worldNumber;
    public int stageNumber;
    public Door door;

    // 当たり判定内に入った時の処理
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<StageEnterContoroller>()
            ?.SetStageCandidate(this);

        Debug.Log($"Stage Area: {worldNumber}-{stageNumber}");
    }

    // 当たり判定外に出た時の処理
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.GetComponent<StageEnterContoroller>()
            ?.ClearStageCandidate(this);
    }


    // TriggerAreaがどこのステージ前にあるかを確認する関数
    public int GetStageNumber()
    {
        return stageNumber;
    }

    // Doorクラスの関数をControllerで使うための関数
    public void DoorOpen()
    {
        door.OpenDoor();
    }

}
