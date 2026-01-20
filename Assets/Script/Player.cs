using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float moveSpeed = 5.0f; // 移動速度
    [SerializeField, Tooltip("移動速度")] private float rotateSpeed = 10.0f; // 向きを変える速さ（補間用
    private Animator animator;  // アニメーション切り替え用

    private Rigidbody m_Rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
        animator = transform.GetComponent<Animator>();
    }
    void Update()
    {
        // 入力取得（WASD / 矢印キー / ゲームパッド）
        float moveX = 
            Input.GetAxisRaw("Horizontal") + Input.GetAxis("Stick_X") + Input.GetAxis("Cross_X"); // A,D or ←,→
        float moveZ = 
            Input.GetAxisRaw("Vertical") + Input.GetAxis("Stick_Y") + Input.GetAxis("Cross_Y");   // W,S or ↑,↓

        // 移動方向
        Vector3 move = new Vector3(moveX, 0, moveZ);

        if (move.sqrMagnitude > 0.001f)
        {
            // 移動処理
            transform.position += move.normalized * moveSpeed * Time.deltaTime;

            // プレイヤーを進行方向に回転させる
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            animator.SetBool("isMoving", true);
        }
        else
        {
            m_Rb.angularVelocity = Vector3.zero;
            m_Rb.linearVelocity = Vector3.zero;

            animator.SetBool("isMoving", false);
        }


    }

    //プレイヤーとブロックの当たり判定
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            if (collision.rigidbody.isKinematic == false)//ブロックが動いているとき
            {
                //Destroy(gameObject);    //破壊
                Block block = collision.gameObject.GetComponent<Block>();
                if (block == null) return;
                // 「ブロックが動いている場合のみ」
                if (block.CheckMove())
                {
                    block.ForceLockStop();
                }
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
            }
        }
    }

    private IEnumerator Delay(float time, System.Action action)
    {
        yield return new WaitForSecondsRealtime(time);
        action?.Invoke();
    }

}
