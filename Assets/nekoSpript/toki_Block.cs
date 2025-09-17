using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class toki_Block : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("GameMNG")] private toki_GameMNG GameMNG; // gameMng
    [SerializeField, Tooltip("移動速度")] private float moveForce = 5.0f;//移動速度
    private bool hit;
    private bool Move;
    private int Movenum;
    private Vector3 pPos;   //プレイヤーの位置
    private Vector3 bPos;   //自身の位置
    private Rigidbody rb;
    private Vector3[] pushDir;    //進行方向（配列化予定）
    private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hit = false;
        // ブロックの位置
        bPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        pushDir = new Vector3[GameMNG.num];
        for (int i = 0; i < GameMNG.num; i++) { pushDir[i]= Vector3.zero; }
        
        Move =false;
        Movenum = 0;

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
                pushDir[Movenum] = (bPos - pPos);
                pushDir[Movenum].y = 0.0f;
                if (Mathf.Abs(pushDir[Movenum].x)>= Mathf.Abs(pushDir[Movenum].z))
                {
                    pushDir[Movenum].z = 0.0f;
                }
                else 
                {
                    pushDir[Movenum].x = 0.0f; 
                }

                pushDir[Movenum] = pushDir[Movenum].normalized;
                Debug.Log("殴った");

                Movenum++;
                if (Movenum > 2) { Movenum=2; }
               
            }
        }
    }
    private void FixedUpdate()
    {
        if(Move)
        {
            Vector3 move = pushDir[Movenum] * moveForce * Time.fixedDeltaTime;
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
            if (!collidedObjects.Contains(collision.gameObject))
            {
                Debug.Log("初めて当たった相手: " + collision.gameObject.name);
                collidedObjects.Add(collision.gameObject);
                Move = false;           //更新終了
                rb.isKinematic = true;  //動かないように
                Debug.Log("壁に当たった");

                //gameMNGの関数呼び出し
                GameMNG.Check();
            }
            
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
        Debug.Log("離れた");
        // 衝突が終わったらリストから削除
        //collidedObjects.Remove(collision.gameObject);
    }

    public void ReleaseStoredForce(int i)
    {
        if (pushDir[i] != Vector3.zero)
        {
            rb.isKinematic = false;
            Move = true;
            Movenum = i;
            Debug.Log(pushDir[i]);
            //rb.AddForce(pushDir, ForceMode.VelocityChange);
        }
    }

    public bool CheckMove()
    {
        if (Move) { return true; }
        return false;
    }
    public void addMovenum()
    {
        Movenum++;
        if(Movenum > 2) { return; }
        ReleaseStoredForce(Movenum);
    }

}
