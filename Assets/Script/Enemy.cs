using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
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
    private Transform target;    // プレイヤー

    private Coroutine lookCoroutine;
    private Vector3 PosTarget;    // 現在の目標地点
    private Block attachedBlock = null;

    private bool isLookingAround = false; // 首振り中か

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameMNG = FindFirstObjectByType<GameMNG>();
        target = GameObject.FindWithTag("Player")?.transform;
        PosTarget = pointB;
    }

    // Update is called once per frame
    void Update()
    {    
        Move();//移動関数

        if (CanSeeTarget()) //索敵範囲内かのチェック
        {
            Debug.Log("プレイヤー発見！ → ゲームオーバー処理へ");
        }

        DetectionMesh detectionMesh = transform.GetComponent<DetectionMesh>();
        detectionMesh.detected = CanSeeTarget();
    }

    //移動関数
    private void Move()
    {
        //ブロックにあったているときor折り返し時辺り見回してるときは移動しない
        if (attachedBlock != null || isLookingAround) { return; }

        // 通常パトロール
        transform.position = Vector3.MoveTowards(transform.position, PosTarget, speed * Time.deltaTime);
        Vector3 dir = (PosTarget - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // 到達したら首振り動作へ
        if (Vector3.Distance(transform.position, PosTarget) < 0.05f)
        {
            lookCoroutine=StartCoroutine(LookAround(false));
        }
    }

    public void StartInfiniteLook()
    {


        // 一回 Look が動いていたら終了
        if (lookCoroutine != null)
            StopCoroutine(lookCoroutine);
        LookAtPlayerInstant();
        lookCoroutine = StartCoroutine(LookAround(true));
    }
    private void LookAtPlayerInstant()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
    // 首を振るコルーチン
    private IEnumerator LookAround(bool _infinite)
    {
        
        isLookingAround = true;

        float angle = 45f; // 左右角度
        float waitTime = 0.2f;

        do
        {
            bool startRight = Random.value > 0.5f;
            float[] pattern = startRight ? new float[] { angle, -angle, angle } : new float[] { -angle, angle, -angle };

            Quaternion startRot = transform.rotation; // 基準は最初の正面

            foreach (float yaw in pattern)
            {
                Quaternion targetRot = startRot * Quaternion.Euler(0, yaw, 0); // 常に正面基準
                yield return SmoothRotateTo(targetRot, 1.2f);
                yield return new WaitForSeconds(waitTime);
            }

            // 中央に戻す
            yield return SmoothRotateTo(startRot, 1.2f);

        } while (_infinite);

        PosTarget = (PosTarget == pointA) ? pointB : pointA;
        isLookingAround = false;
    }

    // スムーズ回転（自然な加減速）
    private IEnumerator SmoothRotateTo(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

            // 微小揺れで自然さを追加
            float jitter = Mathf.Sin(t * Mathf.PI * 2f) * 0.01f;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t + jitter);

            yield return null;
        }
    }

    private bool CanSeeTarget()
    {
        //ブロックに当たっているときは索敵しないorプレイヤーが存在しない
        if (attachedBlock != null||target==null) return false;

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

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // Block に当たったとき
        if (collision.gameObject.CompareTag("Block"))
        {
            Block block = collision.gameObject.GetComponent<Block>();

            // すでに他のBlockにくっついている場合は、衝突したBlockで押しつぶされたとみなしてDestroy
            if (attachedBlock != null && block != attachedBlock)
            {
                Debug.Log("Enemyが他のBlockに押しつぶされた！");
                Destroy(gameObject);
                return;
            }

            // まだくっついていない場合だけ、Attach処理を実行
            if (attachedBlock == null && block != null)
            {
                if (block.CheckMove())
                {
                    attachedBlock = block;
                    AttachToBlock(block);

                    // Blockの表面位置に少しだけ補正
                    ContactPoint contact = collision.contacts[0];
                    transform.position = contact.point + contact.normal * 1.0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attachedBlock != null)
        {
            // Enemy がくっついた Block によって移動中
            if (other.CompareTag("Object") || other.CompareTag("Block"))
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
