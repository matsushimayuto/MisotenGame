using UnityEngine;
using UnityEngine.UIElements;

public class FollowWorld : MonoBehaviour
{
    public Transform target;    // 追従したい対象（元の親）
    private int Cnt;            // 向きカウント
    private Vector3 backDir;    // 後方ベクトル

    private void Start()
    {
    }

    void Update()
    {
        if (transform.name == "SpeedLine(Clone)")
        {
            Vector3 scale = target.localScale;
            float radius = 0.0f;
            Vector3 spawnPos = target.position + backDir * radius;

            switch (Cnt)
            {
                case 0: // X+
                    // 側面へ少しずらす
                    radius = scale.x * 0.9f;
                    spawnPos = target.position - backDir * radius;
                    spawnPos.y += 2.5f;
                    break;
                case 1: // X-
                    radius = scale.x * -0.9f;
                    spawnPos = target.position + backDir * radius;
                    spawnPos.y += 2.5f;
                    break;
                case 2:// Z+
                    radius = scale.z * 0.75f;
                    spawnPos = target.position - backDir * radius;
                    spawnPos.y += 2.5f;
                    break;
                case 3: // Z-
                    scale = target.localScale;
                    radius = scale.z * -0.75f;
                    spawnPos = target.position + backDir * radius;
                    spawnPos.y += 2.5f;
                    break;

            }
            // オフセット適用
            transform.position = spawnPos;
        }
        else
        {
            // プレイヤーの煙追従
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

    }
    /// <summary>
    /// ターゲットを設定
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    /// <summary>
    ///  ターゲットとの相対回転・スケールを設定
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    public void SetTransform(Vector3 rotation, Vector3 scale)
    {
        // 回転方向を設定        
        backDir = rotation;
        Debug.Log("初回" + backDir);

        if (rotation.x == 1.0f)
        {
            Cnt = 0;
            transform.rotation = Quaternion.Euler(90.0f, 0f, 180f);
            transform.localScale = new Vector3(8.0f, 5.0f, 5.0f);
        }
        if (rotation.x == -1.0f)
        {
            Cnt = 1;
            transform.rotation = Quaternion.Euler(90.0f, 0f, 0.0f);
            transform.localScale = new Vector3(8.0f, 5.0f, 5.0f);
        }
        if (rotation.z == 1.0f)
        {
            Cnt = 2;
            transform.rotation = Quaternion.Euler(90.0f, 0f, -90.0f);
            transform.localScale = new Vector3(5.0f, 5.0f, 8.0f);
            if (rotation.z == -1.0f)
            {
                Cnt = 3;
                transform.rotation = Quaternion.Euler(90.0f, 0f, 90.0f);
                transform.localScale = new Vector3(5.0f, 5.0f, 8.0f);
            }
        }
    }
}
