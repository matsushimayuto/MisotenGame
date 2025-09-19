// プレイヤーの弾の動き関連

using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("削除")]
    public float m_fDestroyTime = 10.0f;

    [Header("弾を当てる対象のタグ")]
    public string m_sTargetTag = string.Empty;  // 9/16:敵のタグを一時的に"Respawn"にしているので競合解決後"Enemy"を追加し直すこと

    private float m_fTimeCount = 0.0f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 取得
    }

    // Update is called once per frame
    void Update()
    {
        // フィールドの傾き方向に動かす
        if (Input.GetKeyDown(KeyCode.W)) rb.AddForce(new Vector3(0.0f, 0.0f, 200.0f));    // 前
        if (Input.GetKeyDown(KeyCode.S)) rb.AddForce(new Vector3(0.0f, 0.0f, -200.0f));    // 後
        if (Input.GetKeyDown(KeyCode.A)) rb.AddForce(new Vector3(-200.0f, 0.0f, 0.0f));    // 左
        if (Input.GetKeyDown(KeyCode.D)) rb.AddForce(new Vector3(200.0f, 0.0f, 0.0f));    // 右

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
