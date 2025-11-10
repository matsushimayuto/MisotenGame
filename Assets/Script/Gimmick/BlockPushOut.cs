using UnityEngine;

public class BlockPushOut : MonoBehaviour
{
    [HideInInspector] public bool isActiveBlock = false; // 表示中かどうか
    public float pushForce = 6.0f;                       // 押し出す強さ
    public float pushStep = 0.15f;                       // 1フレームで押す距離
    public string playerTag = "Player";                  // プレイヤーのタグ

    private void OnCollisionStay(Collision collision)
    {
        if (!isActiveBlock) return;
        if (!collision.collider.CompareTag(playerTag)) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        Vector3 playerPos = rb.position;

        // ブロック中心→プレイヤーへのXZ方向
        Vector3 pushDir = playerPos - transform.position;
        pushDir.y = 0f;

        // プレイヤーがブロック中心に完全に重なっている場合は右方向に押す
        if (pushDir.sqrMagnitude < 0.0001f)
            pushDir = Vector3.right;

        pushDir.Normalize();

        // Rigidbodyを直接移動して埋まりを解除
        rb.position += pushDir * pushStep;

        // デバッグ
        Debug.DrawLine(transform.position, rb.position, Color.red, 0.1f);
    }
}
