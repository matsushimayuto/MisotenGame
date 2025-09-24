using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("最初の位置")]private Vector3 pointA;   // パトロール開始地点
    [SerializeField, Tooltip("目的地の位置")] private Vector3 pointB;   // パトロール終了地点
    [SerializeField, Tooltip("移動速度")] private float speed = 2.0f;   // 移動速度
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    private Vector3 target;    // 現在の目標地点
    private Block attachedBlock = null;
    private Vector3 blockNormal = Vector3.zero; // Block 衝突時の法線
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = pointB;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMNG.timestop) { return; }

        if (attachedBlock != null)
        {
            // Block に押されているとき
            Vector3 move = attachedBlock.GetMoveVector() * Time.deltaTime;
            transform.position += move;

            // Block に埋まらないように前回衝突法線方向に少し補正
            transform.position += blockNormal * 0.05f;

            // 正面判定で Object に当たったら Destroy
            if (attachedBlock.GetHitObjectFront())
            {
                Debug.Log("Enemy破壊");
                Destroy(gameObject);
            }

            // 進行方向に回転
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }
        }
        else
        {
            // 通常パトロール
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            Vector3 dir = (target - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, target) < 0.05f)
            {
                target = (target == pointA) ? pointB : pointA;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Block に当たったとき
        if (collision.gameObject.CompareTag("Block"))
        {
            Block block = collision.gameObject.GetComponent<Block>();
            if (block != null)
            {
                attachedBlock = block;
                if (block != null && block.CheckMove())
                {
                    AttachToBlock(block);
                }
            }
        }
    }

    private void AttachToBlock(Block block)
    {
        attachedBlock = block;
        transform.parent = block.transform; // 親子付け
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; // 物理干渉を停止
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true; // 衝突判定を停止
    }
}
