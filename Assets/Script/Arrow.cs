using UnityEngine;

public class Arrow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    const float positionY = 7.3f;   // Y軸の座標
    const float shiftWidth = 1.0f;  // ブロックの中心からどれだけずらして表示するか

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
        switch (_phase)
        {
            case 0: // フェーズ1
                spriteRenderer.color = Color.blue;
                break;  // 色を変えないので何もしない
            case 1: // フェーズ2
                spriteRenderer.color = Color.green;
                break;
            case 2: // フェーズ3
                spriteRenderer.color = Color.red;
                break;
        }

        // 向きを変えて表示
        transform.gameObject.SetActive(true);   // 表示
        //Debug.Log(_pushDir);
        if (_pushDir.x != 0)
        {
            if(_pushDir.x > 0.0f)   // 右
            {
                transform.position = new Vector3(
                    (_blockPos.x + _blockScale.x) * shiftWidth + AdjustByCameraX(_blockPos.x), 
                    positionY, 
                    _blockPos.z + AdjustByCameraZ(_blockPos.z));
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, -90.0f);
            }
            else                    // 左
            {
                transform.position = new Vector3(
                    (_blockPos.x - _blockScale.x) * shiftWidth + AdjustByCameraX(_blockPos.x), 
                    positionY, 
                    _blockPos.z + AdjustByCameraZ(_blockPos.z));
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 90.0f);
            }
        }
        else
        {
            if (_pushDir.z > 0.0f)  // 上
            {
                transform.position = new Vector3(
                    _blockPos.x + AdjustByCameraX(_blockPos.x), 
                    positionY, 
                    (_blockPos.z + _blockScale.z) * shiftWidth + AdjustByCameraZ(_blockPos.z));
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
            else                    // 下
            {
                transform.position = new Vector3(
                    _blockPos.x + AdjustByCameraX(_blockPos.x), 
                    positionY, 
                    (_blockPos.z - _blockScale.z) * shiftWidth + AdjustByCameraZ(_blockPos.z));
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);
            }
        }
    }

    // XとZの表示座標ついて
    // ブロックの真上に出ていてもカメラから見た関係でブロックの位置からずれて見えるため、
    // カメラから見た中心座標を元に表示する位置をずらす
    private float AdjustByCameraX(float blockPosX)
    {
        return (0.0f - blockPosX) * 0.225f;
    }

    private float AdjustByCameraZ(float blockPosZ)
    {
        return (-11.0f - blockPosZ) * 0.20f;
    }
}
