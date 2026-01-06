using UnityEngine;

public class TeleportMirror : MonoBehaviour
{
    [Header("テレポート先の出口オブジェクト")]
    public Transform teleportExit;

    [Header("出口から少し前に出す距離")]
    public float offsetDistance = 0.0f;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Block")) return;

        Block block = collision.gameObject.GetComponent<Block>();
        if (block == null)
            return;

        // === 角度チェック（正面衝突のみ）===
        Vector3 moveDir = block.GetDeltaMove().normalized;
        float dot = Vector3.Dot(moveDir, -transform.up);
        if (dot > -0.9f)
        {
            Debug.Log("角度が違う");
            return;
        }

        // === 相対位置の記録 ===
        // ブロック中心がワープAの中心からどれだけ離れているか（ローカル基準）
        Vector3 localOffsetFromWarpA = transform.InverseTransformPoint(collision.transform.position);

        // === 出口側の位置を計算 ===
        // 入口側の相対位置を、出口側のローカル空間に変換
        Vector3 worldOffsetFromWarpB = teleportExit.TransformPoint(localOffsetFromWarpA);

        // 出口の前に少し出す
        Vector3 targetPos = worldOffsetFromWarpB + teleportExit.forward * offsetDistance;

        // === テレポート先のブロック重なりチェック ===
        if (!CanTeleport(collision.gameObject, teleportExit, targetPos))
        {
            Debug.Log("出口が塞がっているためテレポート中止");
            block.StopMove();
            return;
        }
        // 出口と入口の回転差分を計算してブロックに適用
        Quaternion rotationDelta = teleportExit.rotation * Quaternion.Inverse(transform.rotation);
        collision.transform.rotation = rotationDelta * collision.transform.rotation;

        // === テレポート実行 ===
        collision.transform.position = targetPos;

        Debug.Log($"{collision.gameObject.name} がワープA→Bにテレポートしました！");
    }

    bool CanTeleport(GameObject block, Transform teleportExit, Vector3 targetPos)
    {
        BoxCollider box = block.GetComponent<BoxCollider>();
        if (box == null) return true;

        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, block.transform.lossyScale);
        Vector3 targetCenter = targetPos;

        Collider[] overlaps = Physics.OverlapBox(
            targetCenter,
            halfExtents,
            teleportExit.rotation,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (var hit in overlaps)
        {
            if (hit.gameObject == block) continue;
            if (hit.CompareTag("Block") || hit.CompareTag("Object"))
            {
                Debug.Log("テレポート先がふさがってる: " + hit.name);
                return false;
            }
        }

        return true;
    }

    // ===== Sceneビューで確認用 =====
    private void OnDrawGizmosSelected()
    {
        if (teleportExit == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(teleportExit.position, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, teleportExit.position);
    }
}
