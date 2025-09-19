using UnityEngine;

public class toki_Player : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float moveSpeed = 5.0f; // 移動速度
    [SerializeField, Tooltip("移動速度")] private float rotateSpeed = 10.0f; // 向きを変える速さ（補間用
    
    private Rigidbody m_Rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
       
    }
    void Update()
    {
        // 入力取得（WASD / 矢印キー）
        float moveX = Input.GetAxisRaw("Horizontal"); // A,D or ←,→
        float moveZ = Input.GetAxisRaw("Vertical");   // W,S or ↑,↓

        // 移動方向
        Vector3 move = new Vector3(moveX, 0, moveZ);

        if (move.sqrMagnitude > 0.001f)
        {
            // 移動処理
            transform.position += move.normalized * moveSpeed * Time.deltaTime;

            // プレイヤーを進行方向に回転させる
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

       
    }

    //プレイヤーとブロックの当たり判定
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            if (collision.rigidbody.isKinematic == false)//ブロックが動いているとき
            {
                Destroy(gameObject);    //破壊
            }
        }
    }

}
