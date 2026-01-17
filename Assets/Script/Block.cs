using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("移動速度")] private float moveForce = 5.0f;//移動速度

    [Header("鏡ブロック")]
    [SerializeField, Tooltip("鏡ブロックならチェック")] private bool bMirror=false;//移動速度
    [SerializeField, Tooltip("対となるブロック")] private Block MirrorObj;//移動速度
    private float hitStopTime; // ヒットストップ時間
    private bool isHitStopping; // 現在ヒットストップ中かどうか
    private bool hit;
    private bool bMove;
    private int Movenum;
    private Vector3 pPos;   //プレイヤーの位置
    private Vector3 bPos;   //自身の位置
    private Vector3 bScale; // 自身のサイズ
    private Rigidbody rb;
    private Vector3[] pushDir;    //進行方向（配列化予定）
    private Vector3 deltaMove; // 直近の移動量

    [Header("矢印")]
    [SerializeField, Tooltip("プレハブ")] public GameObject[] arrowPrefab = new GameObject[3];    // 矢印のプレハブ
    private GameObject[] arrowInstance = new GameObject[3];
    private Arrow[] arrow = new Arrow[3];

    [Header("エフェクト")]
    [SerializeField] private GameObject stopEffectPrefab;  // 執事の殴りエフェクト
    [SerializeField] private GameObject SpeedEffectPrefab;  // 速度線エフェクト

    // 執事(仮) ※後々削除予定
    [Header("執事")]
    [SerializeField] private GameObject butlerPrefab;   // 執事(仮)のプレハブ
    private Animator butlerAnim; // アニメーション切り替え用(エネミー)

    const float destroyTime = 1.0f;
    private float timeCount = 0.0f;

    // ゲームパッド関連
    const float resetSec = 0.08f;   // ゲームパッド同時押しの対応秒数(80m秒)
    private float lastLBDownTime = -Mathf.Infinity;     // LBを最後に押した時間
    private float lastRBDownTime = -Mathf.Infinity;     // RB
    private float lastYDownTime  = -Mathf.Infinity;     // Y
    private bool isYDown = false;           // Y(方向指定)ボタンを押したか

    private GameObject effect;      // エフェクト本体
    private FollowWorld follow;     // 速度線エフェクト用
    private GameObject shituji;      // 執事本体 
    private Animator PlayerAnim;    // アニメーション切り替え用(プレイヤー)

    private bool bStartStop = false;
    private bool bStartMove_1 = false;
    private bool bStartMove_2 = false;

    // 外壁の座標データ
    float[,] wallPosition = new float[4, 2] {
        { 22.5f, -1.5f },       // 右
        { -22.5f,  -1.5f },     // 左
        { 0.0f,  15.0f },       // 上           
        { 0.0f, -18.0f }        // 下
    };

    void Start()
    {
        GameMNG = FindFirstObjectByType<GameMNG>();

        hit = false;
        // ブロックの位置
        bPos = transform.position;
        bScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pushDir = new Vector3[GameMNG.num];
        for (int i = 0; i < GameMNG.num; i++) { pushDir[i] = Vector3.zero; }

        hitStopTime = 0.05f;
        isHitStopping = false;

        bMove = false;
        Movenum = 0;


        // 矢印
        for (int i = 0; i < GameMNG.num; i++)
        {
            arrowInstance[i] = Instantiate(arrowPrefab[i], bPos, Quaternion.identity);
            arrow[i] = arrowInstance[i].GetComponent<Arrow>();
        }

        // 執事(仮)
        butlerPrefab = Instantiate(butlerPrefab, new Vector3(999.0f, 999.0f, 999.0f), Quaternion.identity);

        butlerAnim = butlerPrefab.GetComponent<Animator>();
        Debug.Log(butlerAnim);
        PlayerAnim = GameObject.Find("Player(Clone)").GetComponent<Animator>();
    }

    void Update()
    {
        if (hit)
        {
            // 入力検知(入力された時間を記録)
            if (Input.GetKeyUp(KeyCode.P) || Input.GetButtonDown("Specific")) { lastYDownTime = Time.time; }
            if (Input.GetButtonDown("LB")) { lastLBDownTime = Time.time; }
            if (Input.GetButtonDown("RB")) { lastRBDownTime = Time.time; }
        }

        // リセット
        if (lastLBDownTime != -Mathf.Infinity && lastLBDownTime + resetSec > Time.time)
        {
            if (Input.GetButtonDown("RB")) { MoveReset(); }
        }
        if (lastRBDownTime != -Mathf.Infinity && lastRBDownTime + resetSec > Time.time)
        {
            if (Input.GetButtonDown("LB")) { MoveReset(); }
        }

        if(Input.GetKeyUp(KeyCode.V))
        {
            PhaseSkip();
        }

        // 方向指定(一定時間内にLBの入力がなかったため)
        if (lastYDownTime != -Mathf.Infinity && lastYDownTime + resetSec < Time.time)    
        {
            // スキップ機能
            if (Input.GetButton("LB")) { PhaseSkip(); }

            // プレイヤー → ブロック の方向ベクトル
            pushDir[Movenum] = (bPos - pPos);
            pushDir[Movenum].y = 0.0f;

            //ブロックが斜めに行かないように値が大きい方に飛ばす
            if (Mathf.Abs(pushDir[Movenum].x) >= Mathf.Abs(pushDir[Movenum].z))
            {
                Debug.Log(pushDir[Movenum]);
                pushDir[Movenum].z = 0.0f;
            }
            else
            {
                Debug.Log(pushDir[Movenum]);
                pushDir[Movenum].x = 0.0f;
            }

            //正規化
            pushDir[Movenum] = pushDir[Movenum].normalized;
            Debug.Log("殴った");

            // 攻撃アニメーション
            PlayerAnim.SetTrigger("Attack");

            // 矢印の描画
            Debug.Log(Movenum);
            arrow[Movenum].Draw(pushDir[Movenum], bPos, bScale, Movenum);

            if (Movenum == 0)
            {
                AppearButler(0);               
            }

            if (bMirror)
            {
                if (MirrorObj != null)
                {
                    MirrorObj.SetPushDir(pushDir[Movenum]);
                }
            }

            addMovenum(false);

            lastYDownTime = -Mathf.Infinity;
        }
    }

    private void FixedUpdate()
    {
        if (bMove && !isHitStopping)
        {
            // 移動中のアニメーション制御
            if (butlerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
          
            }
            else
            {
                if (CheckReserve(Movenum + 1))
                {
                    if (!bStartMove_1)
                    {
                        butlerAnim.SetTrigger("BeforePunch");
                        bStartMove_1 = true;
                    }
                }
                
            }
            if (!bStartStop)
            {
                StopBlock();
                bStartStop = true;
            }
            // レイ判定
            RayTest();
            // 移動処理
            Vector3 move = pushDir[Movenum] * moveForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            deltaMove = move; // 移動量を保存
            bPos = transform.position;
        }
        else
        {
            deltaMove = Vector3.zero;
        }
        

        // 執事(仮)
        //if(butlerPrefab.transform.position.x != 999.0f)
        //{
        //    timeCount += Time.deltaTime;
        //    if (timeCount > destroyTime)
        //    {
        //        butlerPrefab.transform.position = new Vector3(999.0f, 999.0f, 999.0f);
        //        timeCount = 0.0f;
        //    }            
        //}
    }

    //当たっているとき
    void OnCollisionEnter(Collision collision)
    {
        //プレイヤーがブロックと当たっているか
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = true;
            // プレイヤーの位置を保存
            pPos = collision.transform.position;
        }

        // Enemyに当たった場合のヒットストップ
        if (collision.gameObject.CompareTag("Enemy") && !isHitStopping)
        {
            StartCoroutine(HitStopCoroutine());
        }

        if (collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("当たり判定"+Movenum);
            // 接触点の法線
            Vector3 contactNormal = collision.contacts[0].normal;
            // 自分の進行方向（直前の pushDir）
            Vector3 moveDir = pushDir[Movenum].normalized;

            // 進行方向と法線の内積がマイナス（正面衝突）のときだけ停止処理
            if (Vector3.Dot(moveDir, -contactNormal) > 0.5f)
            {
                Debug.Log("正面から壁に当たった: " + collision.gameObject.name);
                StopMove();
            }
            else
            {
                Debug.Log("壁に擦れただけなので無視");
            }
        }
    }

    // 当たり続けている時
    private void OnCollisionStay(Collision collision)
    {
        //プレイヤーがブロックと当たっているか
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーの位置を更新
            pPos = collision.transform.position;
        }
    }

    //プレイヤーが離れたとき
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = false;
        }
    }

    //このフェーズ中のこのブロックの動き
    public bool ReleaseStoredForce(int i)
    {
        Movenum = i;    //初回用必須
        if (pushDir[i] != Vector3.zero)
        {
            rb.isKinematic = false; //固定化解除
            bMove = true;

            // 動き出す瞬間のエフェクト
            SpawnStopEffect();
            SpeedEffect();
            Animator animator = butlerPrefab.GetComponent<Animator>();
            if(!bStartStop)
            {
                animator.SetTrigger("Attack");
            }
            return true;
        }
        return false;
    }

    //このブロックが動いているかチェック
    public bool CheckMove()
    {
        if (bMove) { return true; }
        return false;
    }

    public void StopMove()
    {
        bMove = false;
        rb.isKinematic = true;//ブロック固定
        Debug.Log("とまった");

        // GameMNGに通知
        GameMNG.Check();
    }

    //フェーズ進行用
    public void addMovenum(bool _move)
    {
        Movenum++;
        if (Movenum > 2) Movenum = 2;
        if (_move) ReleaseStoredForce(Movenum);
    }

    // 直前の移動量を取得
    public Vector3 GetDeltaMove()
    {
        return deltaMove;
    }

    // 矢印削除
    public void DestroyArrow()
    {
        for (int i = 0; i < GameMNG.num; i++)
        {
            Destroy(arrowInstance[i]);
        }
    }
    // 最初のアニメーション用ヒットストップ
    public void StopBlock()
    {
        hitStopTime = 3.0f;
        StartCoroutine(HitStopCoroutine());
        hitStopTime = 0.05f;
        
    }

    // ヒットストップ用コルーチン
    private IEnumerator HitStopCoroutine()
    {
        isHitStopping = true;

        // 一時停止
        rb.isKinematic = true;

        yield return new WaitForSeconds(hitStopTime);

        // 再開
        rb.isKinematic = false;
        isHitStopping = false;
    }

    // 現在のフェーズ取得
    public int GetPhase()
    {
        return Movenum;
    }

    // ブロックに動きが予約されているか
    // 予約されている:true されていない:false
    public bool CheckReserve(int i)
    {
        if(i < 3)
        {
            if (pushDir[i] != Vector3.zero) { return true; }
            else { return false; }
        }
        else
        {
            return false;
        }
    }

    // 動く方向の番号を返す
    // 右:0 左:1 上:2 下:3
    private int GetDirNumber(int Phase)
    {
        if (pushDir[Phase].x != 0)
        {
            if (pushDir[Phase].x > 0.0f)   // 右
            {
                return 0;
            }
            else                    // 左
            {
                return 1;
            }
        }
        else
        {
            if (pushDir[Phase].z > 0.0f)  // 上
            {
                return 2;
            }
            else                    // 下
            {
                return 3;
            }
        }
    }

    private float WhichLeftorRight(float bPos, int dirNum)
    {
        float subPos = bPos - wallPosition[dirNum, 0];
        if (subPos < 0) return bPos;
        else return -bPos;
    }

    private float WhichUporDown(float bPos, int dirNum)
    {
        float subPos = bPos - wallPosition[dirNum, 1];
        if (subPos > 0) return bPos;
        else return -bPos;
    }

    public void SetPushDir(Vector3 _Dir)
    {
        pushDir[Movenum] = -_Dir;
        arrow[Movenum].Draw(pushDir[Movenum], bPos, bScale, Movenum);
        addMovenum(false);
    }

    private void SpawnStopEffect()
    {
        // 今回の進行方向
        Vector3 dir = pushDir[Movenum];

        // 無効なら何もしない
        if (dir == Vector3.zero || stopEffectPrefab == null) return;

        // 反対方向
        Vector3 backDir = -dir;

        // ----------- ブロックの半径を計算 -----------
        Vector3 scale = transform.localScale;   
        float radius;

        // X方向が強い → X側面
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
        {
            radius = scale.x * 0.5f;
        }
        else // Z方向が強い → Z側面
        {
            radius = scale.z * 0.5f;
        }

        // 側面へ少しずらす
        Vector3 spawnPos = transform.position + backDir * radius;
        spawnPos.y += 3.0f;

        // エフェクト生成
        shituji = Instantiate(stopEffectPrefab, spawnPos, Quaternion.LookRotation(backDir));
    }

    // 執事出現用関数
    public void AppearButler(int Phase)
    {
        if(butlerAnim)
        if (pushDir[Phase].x != 0)
        {
            if (pushDir[Phase].x > 0.0f)   // 右
            {
                if(Phase == 0)  // フェーズ1の場合はブロックのそばに出す
                {
                    butlerAnim.SetTrigger("Ojigi");
                    butlerPrefab.transform.position = new Vector3(bPos.x - bScale.x * 1.5f, 1.5f, bPos.z);
                }
                else            // フェーズ2以降は外壁の近くに出す
                {
                    // アニメーション
                    if(PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position = 
                        new Vector3(wallPosition[dirNum, 0] + WhichLeftorRight(bPos.x, dirNum) - bScale.x * 1.5f, 
                        1.5f, wallPosition[dirNum, 1]);
                }                    
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
            else                    // 左
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x + bScale.x * 1.5f, 1.5f, bPos.z);
                }
                else
                {
                    // アニメーション  
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0] + WhichLeftorRight(bPos.x, dirNum) + bScale.x * 1.5f,
                    1.5f, wallPosition[dirNum, 1]);
                }
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, -90.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
        }
        else
        {
            if (pushDir[Phase].z > 0.0f)  // 上
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x, 1.5f, bPos.z - bScale.z * 1.5f);
                }
                else
                {
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0], 1.5f, 
                    wallPosition[dirNum, 1] + WhichUporDown(bPos.z, dirNum) - bScale.z * 1.5f);
                }
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
            else                    // 下
            {
                if (Phase == 0)
                {
                        butlerAnim.SetTrigger("Ojigi");
                        butlerPrefab.transform.position = new Vector3(bPos.x, 1.5f, bPos.z + bScale.z * 1.5f);
                }
                else
                {
                    // アニメーション
                    if (PlayerAnim)
                    {
                        PlayerAnim.SetTrigger("Call");
                    }
                        //if (CheckReserve(Phase + 1))
                        //{
                        //    butlerAnim.SetTrigger("BeforePunch");
                        //}
                        //else
                        //{
                        //    butlerAnim.SetTrigger("AfterPunch");
                        //}
                        int dirNum = GetDirNumber(Phase - 1);
                    butlerPrefab.transform.position =
                    new Vector3(wallPosition[dirNum, 0], 1.5f,
                    wallPosition[dirNum, 1] + WhichUporDown(bPos.z, dirNum) + bScale.z * 1.5f);
                }
                butlerPrefab.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                ChangeEffect(butlerPrefab.GetComponentInChildren<ParticleSystem>());
            }
        }
        Debug.Log(butlerPrefab.transform.position);
    }

    // 速度線エフェクト
    private void SpeedEffect()
    {
        if (transform.Find("SpeedLine"))
        {
            Destroy(transform.Find("SpeedLine").gameObject);
        }

        // 今回の進行方向
        Vector3 dir = pushDir[Movenum];

        // 無効なら何もしない
        if (dir == Vector3.zero || stopEffectPrefab == null) return;

        // 反対方向
        Vector3 backDir = -dir;

        // ----------- ブロックの半径を計算 -----------
        Vector3 scale = transform.localScale;
        Debug.Log(scale);
        float radius;

        // X方向が強い → X側面
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
        {
            radius = scale.x * 0.5f;
        }
        else // Z方向が強い → Z側面
        {
            radius = scale.z * 0.5f;
        }

        // 側面へ少しずらす
        Vector3 spawnPos = transform.position + backDir * radius;

        // エフェクト生成
        if (!effect)
        {
            //エフェクトを生成
            effect = Instantiate(SpeedEffectPrefab, spawnPos, Quaternion.LookRotation(backDir));
            follow = effect.GetComponent<FollowWorld>();
            //ターゲット情報を設定
            follow.SetTarget(this.transform);
            follow.SetTransform(pushDir[Movenum], transform.localScale);
            butlerAnim.SetTrigger("Attack");

        }
        else
        {
            // ターゲット情報を更新
            follow.SetTransform(pushDir[Movenum], transform.localScale);
        }
    }

    private void MoveReset()
    {
        // 触れているブロックの動きをリセットする
        Debug.Log("リセット");
        Reset();
        if (bMirror)
        {
            if (MirrorObj != null)
            {
                MirrorObj.Reset();
            }
        }
        // 連続でtrueを通らないようにタイムスタンプをリセット
        lastLBDownTime = lastRBDownTime = -Mathf.Infinity; 
    }

    public void Reset()
    {
        for (int i = 0; i < GameMNG.num; i++) {
            pushDir[i] = Vector3.zero;
            arrow[i].gameObject.SetActive(false);
        }
        Movenum = 0;
    }

    private void PhaseSkip()
    {  
        // 触れているブロックのフェーズをスキップ
        Debug.Log("スキップ");
        addMovenum(false);
        // 連続でtrueを通らないようにタイムスタンプをリセット
        lastLBDownTime = lastYDownTime = -Mathf.Infinity;
    }

    private void ChangeEffect(ParticleSystem particle)
    {
        particle.Play();
    }

    // 鏡ブロックセッター
    public void SetMirror(Block other)
    {
        bMirror = true;
        MirrorObj = other;
    }

    // アニメーション開始用レイ判定
    private void RayTest()
    {
        Debug.Log(pushDir[Movenum] + "RayTest");
        //Rayの作成　　　　　　　↓Rayを飛ばす原点　　　↓Rayを飛ばす方向
        // 今回の進行方向
        Vector3 dir = pushDir[Movenum];
        Debug.Log(dir);
        Ray ray = new Ray(transform.position, dir);

        //Rayが当たったオブジェクトの情報を入れる箱
        RaycastHit hit;

        //Rayの飛ばせる距離
        int distance = 10;

        //Rayの可視化    ↓Rayの原点　　　　↓Rayの方向　　　　　　　　　↓Rayの色
        Debug.DrawLine(ray.origin, ray.direction * distance, Color.red);

        //もしRayにオブジェクトが衝突したら
        //                  ↓Ray  ↓Rayが当たったオブジェクト ↓距離
        if (Physics.Raycast(ray, out hit, distance))
        {
            //Rayが当たったオブジェクトのtagがObjectだったら
            if (hit.collider.tag == "Object")
            // アニメーション再生開始判定
            if (CheckReserve(Movenum + 2))
            {

            }
            else
            {
                if (!bStartMove_2)
                {
                    bStartMove_2 = true;
                    butlerAnim.SetTrigger("AfterPunch");
                }
            }
        }
    }
}
