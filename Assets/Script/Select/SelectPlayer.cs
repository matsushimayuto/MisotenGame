
using UnityEngine;

public class SelectPlayer : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField, Tooltip("移動速度")] private float moveSpeed = 5.0f; // 移動速度
    [SerializeField, Tooltip("移動速度")] private float rotateSpeed = 10.0f; // 向きを変える速さ（補間用

    private Rigidbody rb;
    private Vector3 prevPosition;
    private float stepDistance = 1.5f;
    float moved = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        float delta = Vector3.Distance(transform.position, prevPosition);
        moved += delta;

        if(moved >= stepDistance)
        {
            PlayerFootStep();
            moved = 0;
        }

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
            rb.MovePosition(rb.position + move.normalized * moveSpeed * Time.fixedDeltaTime);

            // プレイヤーを進行方向に回転させる
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));

        }
        else
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;

        }
        // 過去座標を取得
        prevPosition = transform.position;
    }

    private void PlayerFootStep()
    {
        //AudioManager.Instance.PlaySE("MoveSE");
    }

}
