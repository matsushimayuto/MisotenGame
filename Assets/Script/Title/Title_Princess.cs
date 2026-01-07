using UnityEngine;

public class Title_Princess : MonoBehaviour
{
    private Animator animator;  // アニメーション
    public Camera camera;       // カメラ
    private float timeCount = 0.0f;     // 時間のカウント
    const float reactionTime = 4.5f;    // 後ろを通り過ぎる時間
    const float settingTime = reactionTime + 3.1f;  // 泥棒の前にセッティングする時間
    const float chaseTime = 9.7f;       // 追いかけ始める時間
    const float stopTime = 11.5f;       // アニメーションを止める時間
    private bool bOnce = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        timeCount += Time.deltaTime;

        //---アニメーション関連
        // 泥棒の後ろ通過
        if (timeCount > reactionTime && timeCount < settingTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.position += new Vector3(0.6f, 0.0f, 0.0f);
        }
        // 泥棒の前にセッティング
        if (timeCount > settingTime && !bOnce)
        {
            animator.SetInteger("n_MoveNum", 1);
            gameObject.transform.position = new Vector3(-1.55f, 0.02f, -19.0f);
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            bOnce = true;
        }
        // 泥棒を追いかける
        if(timeCount > chaseTime && timeCount < stopTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -0.08f);
            camera.transform.position += new Vector3(-0.002f, 0.005f, 0.015f);
        }
        // ストップ
        if (timeCount > stopTime)
        {
            animator.speed = 0.0f;
        }
    }
}
