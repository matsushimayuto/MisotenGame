// プレイヤーの挙動

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProtoPlayer : MonoBehaviour
{
    [Header("弾")]
    public GameObject protoBullet;     // 発射する弾のプレハブ
    public float bulletSpeed = 3.0f;   // 弾のスピード

    [Header("フィールドのTransform")]
    public Transform fieldTransform;

    [Header("回転速度設定")]
    public float minSmoothness = 5.0f;   // ゆっくり回転するとき
    public float maxSmoothness = 30.0f;  // 急に回転するとき
    public float sensitivity = 1.0f;     // 傾き変化に対する反応の強さ

    private Vector3 previousTiltDirection = Vector3.zero;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Enterで発射
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Shoot();
        }

        // フィールドの傾き（ローカル回転）を取得
        Vector3 fieldEuler = fieldTransform.localRotation.eulerAngles;

        // Z軸（左右傾き）とX軸（前後傾き）から向きのベクトルを計算
        Vector3 tiltDirection = new Vector3(
            -Mathf.Sin(fieldEuler.z * Mathf.Deg2Rad),
            0.0f,
            Mathf.Sin(fieldEuler.x * Mathf.Deg2Rad)
        );

        // 傾きの変化量を計算
        float tiltChange = Vector3.Distance(tiltDirection, previousTiltDirection);
        previousTiltDirection = tiltDirection;

        // 回転スムーズさを動的に調整
        float dynamicSmoothness = Mathf.Lerp(minSmoothness, maxSmoothness, tiltChange * sensitivity);

        // 傾き方向がゼロでない場合のみ回転
        if (tiltDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tiltDirection, Vector3.up);

            // プレイヤーのY軸だけを回転させる
            Quaternion currentRotation = transform.rotation;
            Quaternion newRotation = Quaternion.Euler(
                currentRotation.eulerAngles.x,
                targetRotation.eulerAngles.y,
                currentRotation.eulerAngles.z
            );

            transform.rotation = Quaternion.Slerp(currentRotation, newRotation, Time.deltaTime * dynamicSmoothness);
        }
    }

    // 弾の生成と発射
    void Shoot()
    {
        // オブジェクトの位置と向きで弾を生成
        GameObject bullet = Instantiate(protoBullet, transform.position, transform.rotation);

        // 弾に力を加える（Rigidbodyが必要）
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
        }
    }
}
