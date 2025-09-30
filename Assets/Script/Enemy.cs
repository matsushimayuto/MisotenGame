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

                    // ここで一度だけBlockの表面に位置を補正
                    ContactPoint contact = collision.contacts[0];
                    transform.position = contact.point + contact.normal * 1.0f; // 少しだけ押し出す
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attachedBlock != null)
        {
            // Enemy がくっついた Block によって移動中
            if (other.CompareTag("Object"))
            {
                // Block の進行方向に対して正面衝突かチェック
                Vector3 moveDir = attachedBlock.GetDeltaMove().normalized;
                Vector3 contactDir = (other.ClosestPoint(transform.position) - transform.position).normalized;

                if (Vector3.Dot(moveDir, contactDir) > 0.5f) // 正面衝突
                {
                    Debug.Log("EnemyがBlockにくっついた状態でObjectに正面衝突");
                    Destroy(gameObject);
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
