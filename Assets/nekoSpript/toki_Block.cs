using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class toki_Block : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("GameMNG")] private toki_GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("GameMNG")] private float moveForce = 5.0f;//移動速度
    private bool hit;
    private bool Move;
    private Vector3 pPos;   //プレイヤーの位置
    private Vector3 bPos;   //自身の位置
    private Rigidbody rb;   
    private Vector3 pushDir;    //進行方向（配列化予定）
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hit = false;
        // ブロックの位置
        bPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pushDir=Vector3.zero;
        Move =false;

    }

    // Update is called once per frame
    void Update()
    {
        bool stop = GameMNG.timestop;
        if (hit && stop)
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                // プレイヤー → ブロック の方向ベクトル
                pushDir = (bPos - pPos);
                pushDir.y = 0.0f;
                if(Mathf.Abs(pushDir.x)>= Mathf.Abs(pushDir.z))
                {
                    pushDir.z = 0.0f;
                }
                else 
                { 
                    pushDir.x = 0.0f; 
                }

                pushDir=pushDir.normalized;
                Debug.Log("殴った");
            }
        }
    }
    private void FixedUpdate()
    {
        if(Move)
        {
            Vector3 move = pushDir * moveForce * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }

    //プレイヤーが当たっているとき
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = true;
            // プレイヤーの位置を保存
            pPos = collision.transform.position;
            Debug.Log("プレイヤーに当たった");
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            Move = false;           
            Debug.Log("壁に当たった");
        }
    }

    //プレイヤーが離れたとき
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = false;
            Debug.Log("離れた");
        }
    }

    public void ReleaseStoredForce()
    {
        if (pushDir != Vector3.zero)
        {
            rb.isKinematic = false;
            Move = true;
            //rb.AddForce(pushDir, ForceMode.VelocityChange);
        }
    }
}
