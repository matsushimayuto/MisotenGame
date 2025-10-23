using UnityEngine;

public class TeleportMirror : MonoBehaviour
{
    [Header("テレポート先の出口オブジェクト")]
    public Transform teleportExit; // 出口のTransformをInspectorで指定

    [Header("テレポート距離のオフセット")]
    public float offsetDistance = 4.0f; // 出口の少し前に出す距離

    [Header("チェックする半径")]
    [SerializeField] private float checkRadius;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("あたった");
        // ぶつかったオブジェクトがブロックか確認
        if (!collision.gameObject.CompareTag("Block")) return;

        // ブロックのスクリプト取得
        Block block = collision.gameObject.GetComponent<Block>();
        if (block == null) return;

        // 接触面の法線を取得（最初の接触点を使用）
        Vector3 contactNormal = collision.contacts[0].normal;

        // ブロックの進行方向を取得
        Vector3 moveDir = block.GetDeltaMove().normalized;
        Debug.Log(moveDir);
        Debug.Log(contactNormal);
        // 正面衝突しているか判定（内積が正なら正面）
       // float dot = Vector3.Dot(moveDir, -contactNormal);
        float dot = Vector3.Dot(moveDir, transform.forward);
        if (dot > -0.9f) // 角度が合わない場合はテレポートしない
        {
            Debug.Log("角度が違う");
            return;
        }

        // テレポート先に他のブロックがあるか判定
        Vector3 targetPos = teleportExit.position + teleportExit.forward * offsetDistance;
        float checkRadius = 4.0f; // ブロックの半径くらい
        Collider[] hitColliders = Physics.OverlapSphere(targetPos, checkRadius);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Block"))
            {
                // 出口にブロックがいる → テレポート中止
                Debug.Log("出口が塞がれているためテレポート中止");
                block.StopMove();
                return;
            }
        }

        // テレポート実行
        collision.transform.position = targetPos;// + teleportExit.forward* offsetDistance;

        // 向きを出口の向きに合わせる
        //collision.transform.rotation = teleportExit.rotation;

        Debug.Log($"{collision.gameObject.name} がテレポートしました！");
    }

    // Scene上で出口のチェック範囲を可視化
    private void OnDrawGizmosSelected()
    {
        if (teleportExit != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 pos = teleportExit.position + teleportExit.forward * offsetDistance;
            Gizmos.DrawWireSphere(pos, checkRadius);
        }
    }
}
