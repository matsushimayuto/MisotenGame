using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("削除")]
    public float m_fDestroyTime = 10.0f;

    [Header("弾を当てる対象のタグ")]
    public string m_sTargetTag = string.Empty;

    private float m_fTimeCount = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 指定時間経ったらオブジェクトを消す
        m_fTimeCount += Time.deltaTime;
        if(m_fTimeCount > m_fDestroyTime)
        {
            Destroy(gameObject);
        }
    }

    // 衝突判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == m_sTargetTag)
        {
            Destroy(collision.gameObject);
        }
    }
}
