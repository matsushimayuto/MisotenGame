using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Thief : MonoBehaviour
{
    private Animator animator;  // アニメーション
    private float timeCount = 0.0f;     // 時間のカウント
    const float reactionTime = 5.0f;    // リアクションさせる時間
    const float rotateTime = 8.7f;      // 回り始める時間
    const float reWalkTime = 9.7f;      // 再び歩き始める時間
    const float stopTime = 11.5f;       // アニメーションを止める時間
    private float walkSpeed = 0.04f;        // 歩くスピード
    private Vector3 startPosition;
    private bool bOnce = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
        startPosition = gameObject.transform.position;
        AudioManager.Instance.PlayBGM("WalkBGM");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        timeCount += Time.deltaTime;

        //---アニメーション関連
        // リアクション前
        if (timeCount < reactionTime)
        {
            gameObject.transform.position += new Vector3(0.0f, 0.0f, walkSpeed);
        }
        // リアクション
        if (timeCount > reactionTime && !bOnce)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySE("ReactionSE");
            animator.SetInteger("n_MoveNum", 1);
            bOnce = true;
        }
        // 回り始める
        if (timeCount > rotateTime && timeCount < reWalkTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.Rotate(new Vector3(0.0f, 4.0f, 0.0f));
        }
        // 歩き始める
        if (timeCount > reWalkTime && timeCount < stopTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            animator.speed = 2.0f;
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -walkSpeed * 2.0f);
        }
        // ストップ
        if (timeCount > stopTime)
        {
            animator.speed = 0.0f;
        }
    }
}
