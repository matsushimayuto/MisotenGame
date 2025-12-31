using UnityEngine;

public class TwoHitEnemy : Enemy
{
    private bool firstHit = false;
    // Block を少し押し戻す距離
    [SerializeField] private float pushBackDistance = 0.5f;

    protected override void OnCollisionEnter(Collision collision)
    {
        // Block 以外は親に任せる
        if (!collision.gameObject.CompareTag("Block"))
        {
            base.OnCollisionEnter(collision);
            return;
        }

        Block block = collision.gameObject.GetComponent<Block>();
        if (block == null) return;

        // 1回目ヒット
        if (!firstHit)
        {
            // 正面衝突のみ有効にする
            Vector3 moveDir = block.GetDeltaMove().normalized;
            Vector3 contactDir = (transform.position - collision.contacts[0].point).normalized;

            if (Vector3.Dot(moveDir, contactDir) < 0.5f)
                return; // 正面じゃないので無効

            firstHit = true;

            block.StopMove();   // Block だけ止める

            // 進行方向と逆に少し戻す
            block.transform.position -= moveDir * pushBackDistance;
            Debug.Log("TwoHitEnemy：1回目ヒット");

            return; // Attachさせない
        }

        // 2回目
        // ここからは「通常 Enemy」と同じ挙動
        base.OnCollisionEnter(collision);
    }
}
