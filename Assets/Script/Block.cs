using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("移動速度")] private float moveForce = 5.0f;//移動速度
    private bool hit;
    private bool Move;
    private bool hitObjectFront;
    private int Movenum;
    private Vector3 pPos;   //プレイヤーの位置
    private Vector3 bPos;   //自身の位置
    private Rigidbody rb;
    private Vector3[] pushDir;    //進行方向（配列化予定）
    private float hitFrontTimer = 0f;
    private float hitFrontDuration = 0.1f; // true を維持する時間（秒）

    void Start()
    {
        hit = false;
        // ブロックの位置
        bPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pushDir = new Vector3[GameMNG.num];
        for (int i = 0; i < GameMNG.num; i++) { pushDir[i] = Vector3.zero; }

        Move = false;
        Movenum = 0;

        hitObjectFront = false;
    }

    void Update()
    {
        if (hitFrontTimer > 0f)
        {
            hitFrontTimer -= Time.deltaTime;
            if (hitFrontTimer <= 0f)
            {
                hitObjectFront = false;
            }
        }

        bool stop = GameMNG.timestop;
        if (hit && stop)
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                // プレイヤー → ブロック の方向ベクトル
                pushDir[Movenum] = (bPos - pPos);
                pushDir[Movenum].y = 0.0f;

                //ブロックが斜めに行かないように値が大きい方に飛ばす
                if (Mathf.Abs(pushDir[Movenum].x) >= Mathf.Abs(pushDir[Movenum].z))
                {
                    pushDir[Movenum].z = 0.0f;
                }
                else
                {
                    pushDir[Movenum].x = 0.0f;
                }

                //正規化
                pushDir[Movenum] = pushDir[Movenum].normalized;
                Debug.Log("殴った");

            }
        }
    }
    private void FixedUpdate()
    {
        if (Move)
        {
            Vector3 move = pushDir[Movenum] * moveForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
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
                Move = false;
                rb.isKinematic = true;//ブロック固定

                hitObjectFront = true;
                hitFrontTimer = hitFrontDuration;

                // GameMNGに通知
                GameMNG.Check();
            }
            else
            {
                Debug.Log("壁に擦れただけなので無視");
            }
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
    public void ReleaseStoredForce(int i)
    {
        Movenum = i;    //初回用必須
        if (pushDir[i] != Vector3.zero)
        {
            rb.isKinematic = false; //固定化解除
            Move = true;
            //Movenum = i;    //初回用必須
            //hitObjectFront = false;
        }
    }

    //このブロックが動いているかチェック
    public bool CheckMove()
    {
        if (Move) { return true; }
        return false;
    }

    //フェーズ進行用
    public void addMovenum(bool _TimeStop)
    {

        Movenum++;
        Debug.Log("加算時" + Movenum);
        if (Movenum > 2|| _TimeStop) { return; }
        ReleaseStoredForce(Movenum);
    }

    // 正面衝突して Object に当たったかどうか
    public bool GetHitObjectFront()
    {
        return hitObjectFront; 
    }

    // 現在の移動方向を取得
    public Vector3 GetMoveVector()
    {
        if (Movenum < 0 || Movenum >= pushDir.Length)
            return Vector3.zero;
        return pushDir[Movenum];
    }
}
