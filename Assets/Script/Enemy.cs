using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using NUnit.Framework.Constraints;

public class Enemy : MonoBehaviour
{
    private Vector3 NextPoint;   // 次の移動先
    [SerializeField, Tooltip("移動量")] private Vector3[] movePoint;   //移動量
    private int PointNum;
    private int dir;

    [SerializeField, Tooltip("移動速度")] private float speed = 2.0f;   // 移動速度
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [Header("視界設定")]
    [SerializeField, Tooltip("視野角")] private float viewAngle = 90.0f;               // 視野角
    [SerializeField, Tooltip("視認距離")] private float viewDistance = 10.0f;          // 視認距離
    //[SerializeField, Tooltip("障害物レイヤー")] private LayerMask obstacleMask;      // 障害物レイヤー
    [SerializeField, Tooltip("エフェクト")] private GameObject hitEffect;    // ヒットエフェクト
    private Transform target;    // プレイヤー
    private Animator EnemyAnimator; // アニメーション切り替え用
    private GameObject EnemyHead; // 敵の頭部
    private DetectionMesh Detection;   // 検知範囲オブジェクト

    private Coroutine lookCoroutine;
  
    private Block attachedBlock = null;
    private Rigidbody rb;

    private bool isLookingAround = false; // 首振り中か
    private bool gameOver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameMNG = FindFirstObjectByType<GameMNG>();
        target = GameObject.FindWithTag("Player")?.transform;
        PointNum = 0;
        NextPoint = movePoint[PointNum] + transform.position; //次の移動先決定
        dir = 1;
        gameOver = false;

        EnemyAnimator = GetComponent<Animator>();
        EnemyHead = transform.Find("group/Head").gameObject;
    }

    // Update is called once per frame
    void Update()
    {    
        Move();//移動関数

        if (CanSeeTarget()) //索敵範囲内かのチェック
        {
            if (!gameOver)
            {
                gameOver = true;
                Time.timeScale = 0.0f;
                GameManager.Instance.ChangeState(GameState.GameOver);
                // Start表示中の遅延処理
                StartCoroutine(Delay(3.0f, () =>
                {
                    // ゲームを再開
                    Time.timeScale = 1f;
                    // StartUIを非表示にする
                    GameManager.Instance.IsFirstStageEnter = false;
                    // シーン遷移
                    SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2.0f);
                }));
                Debug.Log("プレイヤー発見！ → ゲームオーバー処理へ");
            }
        }

        Detection = transform.GetComponent<DetectionMesh>();
        Detection.detected = CanSeeTarget();
    }

    //移動関数
    private void Move()
    {
        //ブロックにあったているときor折り返し時辺り見回してるときは移動しない
        if (attachedBlock != null || isLookingAround) { return; }

        // 通常パトロール
        transform.position = Vector3.MoveTowards(transform.position, NextPoint, speed * Time.deltaTime);
        Vector3 dir = (NextPoint - transform.position).normalized;

        if (dir.sqrMagnitude > 0.001f)
        {
            EnemyAnimator.SetBool("isMoving", true);
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // 到達したら首振り動作へ
        if (Vector3.Distance(transform.position, NextPoint) < 0.05f)
        {
            EnemyAnimator.SetBool("isMoving", false);
            EnemyAnimator.SetTrigger("keikai");
            lookCoroutine = StartCoroutine(LookAround(false));
            NextPointSet();
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

       // NextPoint = (NextPoint == pointA) ? pointB : pointA;
        isLookingAround = false;
    }
    private void NextPointSet()
    {
        if (movePoint.Length == 1)
        {
            dir = (dir == 1) ? -1 : 1;
            NextPoint = (movePoint[PointNum] * dir) + transform.position;
        }
        else
        {
            PointNum += dir;
            NextPoint = (movePoint[PointNum] * dir) + transform.position;
            if ((0 == PointNum && dir == -1) || (movePoint.Length - 1 == PointNum && dir == 1))
            {
                PointNum += dir;
                dir = (dir == 1) ? -1 : 1;
            }
        }
    }

    // スムーズ回転（自然な加減速）
    private IEnumerator SmoothRotateTo(Quaternion targetRot, float duration)
    {
        //Quaternion startRot = transform.rotation;
        //float elapsed = 0f;

        //while (elapsed < duration)
        //{
        //    elapsed += Time.deltaTime;
        //    float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));

        //    // 微小揺れで自然さを追加
        //    float jitter = Mathf.Sin(t * Mathf.PI * 2f) * 0.01f;
        //    transform.rotation = Quaternion.Slerp(startRot, targetRot, t + jitter);

        //    yield return null;
        //}
        Quaternion startRot = transform.rotation;
        Transform EnemyHead = transform.Find("group/Armature/Body").gameObject.transform;
        Transform Detection_Transform = Detection.GetComponent<Transform>();
        bool reached = true;       // 回転中フラグ

        while (reached)
        {
            if (!EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("keikai"))
            {
                reached = false;
            }
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
            EnemyAnimator.SetTrigger("Burst_Before");

            // すでに他のBlockにくっついている場合は、衝突したBlockで押しつぶされたとみなしてDestroy
            if (attachedBlock != null && block != attachedBlock)
            {
                Debug.Log("Enemyが他のBlockに押しつぶされた！");
                attachedBlock.StopBlock();
                StartCoroutine(HitStopCoroutine());
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

                    Vector3 EffectPos = transform.position;
                    EffectPos.y += 2.5f; // 少し上からRayを飛ばす
                    Instantiate(hitEffect, EffectPos, transform.rotation);    // エフェクト生成

                    // 吹っ飛びアニメーション
                    EnemyAnimator.SetTrigger("Burst");
                    attachedBlock.StopBlock();
                    StartCoroutine(HitStopCoroutine());

                    //Destroy(gameObject);
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

    private IEnumerator Delay(float time, System.Action action)
    {
        yield return new WaitForSecondsRealtime(time);
        action?.Invoke();
    }
    // ヒットストップ用コルーチン
    private IEnumerator HitStopCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

    private IEnumerator SmoothRotateToQuaternion(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
