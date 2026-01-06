using UnityEngine;

public class Title_Thief : MonoBehaviour
{
    private Animator animator;  // アニメーション
    private float timeCount = 0.0f;     // 時間のカウント
    const float reactionTime = 5.0f;    // リアクションさせる時間
    const float rotateTime = 7.5f;      // 回り始める時間
    const float reWalkTime = 8.5f;      // 再び歩き始める時間
    private float walkSpeed = 0.01f;        // 歩くスピード

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
    }

    // Update is called once per frame
    void Update()
    {
        timeCount += Time.deltaTime;

        //---アニメーション関連
        // リアクション前
        if (timeCount < reactionTime)
        {
            gameObject.transform.position += new Vector3(0.0f, 0.0f, walkSpeed);
        }
        // リアクション
        if (timeCount > reactionTime)
        {
            animator.SetInteger("n_MoveNum", 1);
        }
        // 回り始める
        if (timeCount > rotateTime && timeCount < reWalkTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f));
        }
        // 歩き始める
        if (timeCount > reWalkTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -walkSpeed);
        }
    }
}
