using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowWorld : MonoBehaviour
{
    private Transform target;    // 追従したい対象（元の親）
    private int Cnt;            // 向きカウント
    private Vector3 backDir;    // 後方ベクトル

    private Renderer targetRenderer;
    private Renderer effectRenderer;

    [SerializeField] private float heightOffset = 6.0f;
    private float ObjectHeight = 5.0f;

    private void Start()
    {
        effectRenderer = transform.GetComponent<Renderer>();

    }

    void Update()
    {
        if (transform.name == "SpeedLine(Clone)")
        {
            if (target == null) return;
            Vector3 scale = target.localScale;
            float radius = 0.0f;
            Vector3 spawnPos = target.position + backDir * radius;

            if (target == null) return;

            // Renderer取得（1回だけ）
            if (targetRenderer == null)
                targetRenderer = target.GetComponent<Renderer>();

            // 半径計算
            float targetRadius = targetRenderer.bounds.extents.x;
            float effectRadius = effectRenderer.bounds.extents.z;

            Debug.Log("ターゲット半径:" + targetRadius + " エフェクト半径:" + effectRadius);
            switch (Cnt)
            {
                case 0: // X+
                    // 側面へ少しずらす
                    radius =  effectRenderer.bounds.extents.x + targetRenderer.bounds.extents.x - 2.0f;
                    Debug.Log("Radius:" + radius);
                    spawnPos = target.position;
                    spawnPos.x -= radius;
                    spawnPos.y = ObjectHeight;
                    break;
                case 1: // X-
                    radius = effectRenderer.bounds.extents.x + targetRenderer.bounds.extents.x - 2.0f;
                    spawnPos = target.position;
                    spawnPos.x += radius;
                    spawnPos.y = ObjectHeight;
                    break;
                case 2:// Z+
                    radius = effectRenderer.bounds.extents.z + targetRenderer.bounds.extents.z;
                    spawnPos = target.position;
                    spawnPos.z -= radius;
                    spawnPos.y = ObjectHeight;
                    break;
                case 3: // Z-
                    radius = effectRenderer.bounds.extents.z + targetRenderer.bounds.extents.z;
                    spawnPos = target.position;
                    spawnPos.z += radius;
                    spawnPos.y = ObjectHeight;
                    break;

            }
            // オフセット適用
            transform.position = spawnPos;
        }
        else
        {
            // ----- 砂煙用 -----
            // 初期位置をターゲットに合わせる
            if (target == null)
            {
                if (GameObject.Find("Player(Clone)") != null)
                {
                    target = GameObject.Find("Player(Clone)").transform;
                }
                else
                {
                    return;
                }
            }
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
        }
        if (rotation.z == -1.0f)
        {
            Cnt = 3;
            transform.rotation = Quaternion.Euler(90.0f, 0f, 90.0f);
            transform.localScale = new Vector3(5.0f, 5.0f, 8.0f);
        }
        CheckRotation(rotation);
    }


    //エフェクトの回転チェック用関数
    private void CheckRotation(Vector3 Rot)
    {
        if(Rot.y != 0) // Y軸回転があった場合
        {
            switch(Cnt)
                {
                case 0:
                    Cnt = 2;
                    transform.rotation = Quaternion.Euler(90.0f, 0f, -90.0f);
                    break;
                case 1:
                    Cnt = 3;
                    transform.rotation = Quaternion.Euler(90.0f, 0f, 90.0f);
                    break;
                case 2:
                    Cnt = 1;
                    transform.rotation = Quaternion.Euler(90.0f, 0f, 180.0f);
                    break;
                case 3:
                    Cnt = 0;
                    transform.rotation = Quaternion.Euler(90.0f, 0f, 0.0f);
                    break;
            }
        }
        else // Y軸回転がなかった場合
        {
            return;
        }
    }
}
