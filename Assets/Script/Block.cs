using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("移動速度")] private float moveForce = 5.0f;//移動速度
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
    [SerializeField, Tooltip("プレハブ")] public GameObject arrowPrefab;    // 矢印のプレハブ
    private GameObject[] arrowInstance = new GameObject[3];
    private Arrow[] arrow = new Arrow[3];

    void Start()
    {
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
            arrowInstance[i] = Instantiate(arrowPrefab, bPos, Quaternion.identity);
            arrow[i] = arrowInstance[i].GetComponent<Arrow>();
        }
    }

    void Update()
    {
        if (hit)
        {
            if (Input.GetKeyUp(KeyCode.P) || Input.GetButtonDown("Specific"))   // キーボード(P) or パッド(Y)
            {
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

                // 矢印の描画
                Debug.Log(Movenum);
                arrow[Movenum].Draw(pushDir[Movenum], bPos, bScale, Movenum);

                addMovenum(false);
            }
        }
    }
    private void FixedUpdate()
    {
        if (bMove && !isHitStopping)
        {
            Vector3 move = pushDir[Movenum] * moveForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            deltaMove = move; // 移動量を保存
        }
        else
        {
            deltaMove = Vector3.zero;
        }
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
}
