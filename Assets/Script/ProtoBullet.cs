using UnityEngine;

public class ProtoBullet : MonoBehaviour
{
    public GameObject protoBullet;      // 発射する弾のプレハブ
    public float bulletSpeed = 10.0f;   // 弾のスピード
    public bool isPlayer;

    private float time;

    const float IntervalTime = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = IntervalTime;    // 発射可能
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {   // プレイヤー
            // Enterで発射
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Shoot();
            }
        }
        else
        {   // 敵
            // 2秒間隔で発射
            if(time > IntervalTime)
            {
                Shoot();
                time = 0.0f;
            }
        }
        time += Time.deltaTime;
    }

    void Shoot()
    {
        // プレイヤーの位置と向きで弾を生成
        GameObject bullet = Instantiate(protoBullet, transform.position, transform.rotation);

        // 弾に力を加える（Rigidbodyが必要）
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
        }
    }

}
