using UnityEngine;

public class ProtoBullet : MonoBehaviour
{
    [Header("弾")]
    public GameObject protoBullet;     // 発射する弾のプレハブ
    public float bulletSpeed = 3.0f;   // 弾のスピード
    public float InitialInterval = 0.0f;    // 最初のインターバルタイム

    private float time;
    private Vector3 direction;
    GameObject target;

    const float IntervalTime = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = InitialInterval;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // 2秒間隔で発射
        if(time > IntervalTime)
        {
            Shoot();
            time = 0.0f;
        }
        
        time += Time.deltaTime;
    }

    void Shoot()
    {
        if (target != null)
        {
            // 発射するベクトルを計算
            direction = (target.transform.position - transform.position).normalized;

            // オブジェクトの位置と向きで弾を生成
            GameObject bullet = Instantiate(protoBullet, transform.position, Quaternion.identity);

            // 弾に力を加える（Rigidbodyが必要）
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * bulletSpeed;
            }
        }
    }

}
