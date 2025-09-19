using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [SerializeField, Tooltip("最初の位置")]private Vector3 pointA;   // パトロール開始地点
    [SerializeField, Tooltip("目的地の位置")] private Vector3 pointB;   // パトロール終了地点
    [SerializeField, Tooltip("移動速度")] private float speed = 2.0f;   // 移動速度
    [SerializeField, Tooltip("GameMNG")] private GameMNG GameMNG; // gameMng
    private Vector3 target;    // 現在の目標地点
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = pointB;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMNG.timestop) { return; }

        // 目標に向かって移動
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 進行方向を向かせる
        Vector3 dir = (target - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f) // 方向があるときだけ回転
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        // 目標に着いたら反転
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            if (collision.rigidbody.isKinematic == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
