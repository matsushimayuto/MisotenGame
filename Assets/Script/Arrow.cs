using UnityEngine;

public class Arrow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    const float shiftWidth = 2.0f;  // ブロックの中心からどれだけずらして表示するか
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 矢印の描画
    public void Draw(Vector3 _pushDir, Vector3 _blockPos, Vector3 _blockScale, int _phase)
    {
        // 色変更
        switch(_phase)
        {
            case 0: // フェーズ1
                break;  // 色を変えないので何もしない
            case 1: // フェーズ2
                spriteRenderer.color = Color.blue;
                break;
            case 2: // フェーズ3
                spriteRenderer.color = Color.green;
                break;
        }

        // 向きを変えて表示
        transform.gameObject.SetActive(true);   // 表示
        Debug.Log(_pushDir);
        if (_pushDir.x != 0)
        {
            if(_pushDir.x > 0.0f)   // 右
            {
                transform.position = new Vector3(_blockPos.x + _blockScale.x * shiftWidth, 4.6f, _blockPos.z);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
            else                    // 左
            {
                transform.position = new Vector3(_blockPos.x - _blockScale.x * shiftWidth, 4.6f, _blockPos.z);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);
            }
        }
        else
        {
            if (_pushDir.z > 0.0f)  // 上
            {
                transform.position = new Vector3(_blockPos.x, 4.6f, _blockPos.z + _blockScale.z * shiftWidth);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 90.0f);
            }
            else                    // 下
            {
                transform.position = new Vector3(_blockPos.x, 4.6f, _blockPos.z - _blockScale.z * shiftWidth);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, -90.0f);
            }
        }
    }
}
