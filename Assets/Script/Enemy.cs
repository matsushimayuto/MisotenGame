using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("最初の位置")]private Vector3 pointA;   // パトロール開始地点
    [SerializeField, Tooltip("目的地の位置")] private Vector3 pointB;   // パトロール終了地点
    [SerializeField, Tooltip("移動速度")] private float speed = 2.0f;   // 移動速度
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [Header("視界設定")]
    [SerializeField, Tooltip("視野角")] private float viewAngle = 90.0f;               // 視野角
    [SerializeField, Tooltip("視認距離")] private float viewDistance = 10.0f;          // 視認距離
    //[SerializeField, Tooltip("障害物レイヤー")] private LayerMask obstacleMask;      // 障害物レイヤー
    [SerializeField, Tooltip("探す対象（プレイヤー）")] private Transform target;    // プレイヤー

    private Vector3 PosTarget;    // 現在の目標地点
    private Block attachedBlock = null;

    [Header("索敵範囲可視化")]
    [SerializeField] private Material detectionMat;  // 半透明表示用マテリアル
    private Mesh detectionMesh;
    private GameObject detectionObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PosTarget = pointB;

        // 索敵範囲の子オブジェクトを作成
        detectionObj = new GameObject("DetectionRange");
        detectionObj.transform.parent = transform;
        detectionObj.transform.localPosition = Vector3.zero;

        // マテリアル設定（指定がなければ Unlit を使用）
        MeshFilter mf = detectionObj.AddComponent<MeshFilter>();
        MeshRenderer mr = detectionObj.AddComponent<MeshRenderer>();
        mr.material = detectionMat != null
            ? detectionMat
            : new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        detectionMesh = new Mesh();
        mf.mesh = detectionMesh;

        // メッシュを初期生成
        UpdateDetectionMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMNG.timestop) { return; }

        if (attachedBlock != null) { return; }
            
        Move();//移動関数
        if (CanSeeTarget()) //索敵範囲内かのチェック
        {
            Debug.Log("プレイヤー発見！ → ゲームオーバー処理へ");
        }

        bool detected = CanSeeTarget();

    // 発見したら視界色を変える
    detectionObj.GetComponent<MeshRenderer>().material.color = 
        detected ? new Color(1, 0, 0, 0.3f) : new Color(1, 1, 0, 0.2f);

    // 毎フレーム視界更新
    UpdateDetectionMesh();


    }

    //移動関数
    private void Move()
    {
        // 通常パトロール
        transform.position = Vector3.MoveTowards(transform.position, PosTarget, speed * Time.deltaTime);
        Vector3 dir = (PosTarget - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, PosTarget) < 0.05f)
        {
            PosTarget = (PosTarget == pointA) ? pointB : pointA;
        }
    }

    private bool CanSeeTarget()
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 1. 距離チェック
        if (distanceToTarget > viewDistance) return false;

        // 2. 扇形（視野角チェック）
        float angle = Vector3.Angle(transform.forward, dirToTarget);
        if (angle > viewAngle * 0.5f) return false;

        // 3. Raycastで障害物チェック
        if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, viewDistance))
        {
            if (hit.transform == target) return true;
        }

        return false;
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

    // Sceneビューで扇形を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }

    //索敵範囲（扇形メッシュ）の生成処理
    private void UpdateDetectionMesh()
    {
        int segments = 100; // 扇形を細かくするほど滑らか
        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero; // 中心点（敵位置）

        float halfAngle = viewAngle * 0.5f;
        float startAngle = -halfAngle;
        float angleStep = viewAngle / segments;

        // 視界メッシュをRaycastで補正
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rot * Vector3.forward;
            // Raycastで障害物に当たったらその地点まで
            Vector3 rayOrigin = transform.position + Vector3.up * 0.01f; // 少し上から打つ
            if (Physics.Raycast(rayOrigin, transform.rotation * dir, out RaycastHit hit, viewDistance))
            {
                vertices[i + 1] = detectionObj.transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertices[i + 1] = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * viewDistance;
            }
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        detectionMesh.Clear();
        detectionMesh.vertices = vertices;
        detectionMesh.triangles = triangles;
        detectionMesh.RecalculateNormals();
    }

}
