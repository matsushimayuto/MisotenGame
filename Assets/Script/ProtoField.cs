// フィールドの動き関連

using UnityEngine;

public class ProtoField : MonoBehaviour
{
    [Header("傾き設定")]
    public float maxTiltAngle = 30.0f;       // 最大傾き角度（度）
    public float tiltSpeed = 50.0f;          // 傾き速度（度/秒）
    public float returnSpeed = 20.0f;        // 中央に戻る速度

    private float currentTiltX = 0.0f;       // 前後の傾き（X軸）
    private float currentTiltZ = 0.0f;       // 左右の傾き（Z軸）

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // WASD入力取得
        float vertical = Input.GetKey(KeyCode.W) ? 1.0f : Input.GetKey(KeyCode.S) ? -1.0f : 0.0f;
        float horizontal = Input.GetKey(KeyCode.D) ? 1.0f : Input.GetKey(KeyCode.A) ? -1.0f : 0.0f;

        // 前後の傾き（X軸）
        if (Mathf.Abs(vertical) > 0.01f)
        {
            currentTiltX += vertical * tiltSpeed * Time.deltaTime;
            currentTiltX = Mathf.Clamp(currentTiltX, -maxTiltAngle, maxTiltAngle);
        }
        else
        {
            currentTiltX = Mathf.MoveTowards(currentTiltX, 0f, returnSpeed * Time.deltaTime);
        }

        // 左右の傾き（Z軸）
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            currentTiltZ += horizontal * tiltSpeed * Time.deltaTime;
            currentTiltZ = Mathf.Clamp(currentTiltZ, -maxTiltAngle, maxTiltAngle);
        }
        else
        {
            currentTiltZ = Mathf.MoveTowards(currentTiltZ, 0f, returnSpeed * Time.deltaTime);
        }

        // 傾きを反映（X軸：前後、Z軸：左右）
        transform.localRotation = Quaternion.Euler(currentTiltX, 0f, -currentTiltZ);

    }
}
