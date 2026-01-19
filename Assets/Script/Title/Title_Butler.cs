using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Butler : MonoBehaviour
{
    [SerializeField, Tooltip("セッティングする座標")] private Vector3 position;

    private Animator animator;  // アニメーション
    private float timeCount = 0.0f;     // 時間のカウント
    const float settingTime = 7.6f;     // 泥棒の前にセッティングする時間
    const float chaseTime = 9.7f;       // 追いかけ始める時間
    const float stopTime = 11.5f;       // アニメーションを止める時間
    private bool[] bOnce = new bool[3] { false, false, false };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
        animator.SetTrigger("Newtral");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        timeCount += Time.deltaTime;

        //---アニメーション関連
        // 泥棒の前にセッティング
        if (timeCount > settingTime && !bOnce[0])
        {
            animator.SetTrigger("Bowing");
            animator.speed = 1.2f;
            gameObject.transform.position = position;
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            bOnce[0] = true;
        }
        // 泥棒を追いかける
        if (timeCount > chaseTime && timeCount < stopTime)
        {
            if (!bOnce[1]) { animator.SetTrigger("Newtral"); bOnce[1] = true; }
            animator.speed = 1.0f;
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -0.08f);
        }
        // ストップ
        if (timeCount > stopTime)
        {
            if (!bOnce[2]) { animator.SetTrigger("Bowing"); bOnce[2] = true; }
        }
    }
}
