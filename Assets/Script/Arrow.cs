using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ñÓàÛÇÃï`âÊ
    public void Draw(Vector3 _pushDir, Vector3 _blockPos, Vector3 _blockScale)
    {
        transform.gameObject.SetActive(true);
        Debug.Log(_pushDir);
        if (_pushDir.x != 0)
        {
            if(_pushDir.x > 0.0f)   // âE
            {
                transform.position = new Vector3(_blockPos.x + _blockScale.x * 2.0f, 4.6f, _blockPos.z);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                Debug.Log(transform.rotation);
                Debug.Log("âE");
            }
            else                    // ç∂
            {
                transform.position = new Vector3(_blockPos.x - _blockScale.x * 2.0f, 4.6f, _blockPos.z);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 180.0f);
                Debug.Log("ç∂");
            }
        }
        else
        {
            if (_pushDir.z > 0.0f)  // è„
            {
                transform.position = new Vector3(_blockPos.x, 4.6f, _blockPos.z + _blockScale.z * 2.0f);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 90.0f);
                Debug.Log("è„");
            }
            else                    // â∫
            {
                transform.position = new Vector3(_blockPos.x, 4.6f, _blockPos.z - _blockScale.z * 2.0f);
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, -90.0f);
                Debug.Log("â∫");
            }
        }
    }
}
