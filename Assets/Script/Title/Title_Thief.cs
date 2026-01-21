using UnityEngine;

public class Title_Thief : MonoBehaviour
{
    [SerializeField, Tooltip("びっくりエフェクト")] private GameObject surprisedEffect;
    [SerializeField, Tooltip("はてなエフェクト")] private GameObject questionEffect;

    private Animator animator;  // アニメーション
    private float timeCount = 0.0f;     // 時間のカウント
    const float reactionTime = 5.0f;    // リアクションさせる時間
    const float rotateTime = 8.7f;      // 回り始める時間
    const float reWalkTime = 9.7f;      // 再び歩き始める時間
    const float stopTime = 11.5f;       // アニメーションを止める時間
    private float walkSpeed = 0.04f;        // 歩くスピード
    private bool[] bOnce = new bool[5] { false, false, false, false, false };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
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
            if (!bOnce[0]) { animator.SetTrigger("Walk"); bOnce[0] = true; }
        }
        // ?
        if (timeCount > reactionTime && !bOnce[1])
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySE("QuestionSE");
            animator.SetTrigger("Reaction");
            Instantiate(questionEffect, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
            bOnce[1] = true;
        }
        // !
        if (timeCount > reactionTime + 3.1f && !bOnce[4])
        {
            AudioManager.Instance.PlaySE("ReactionSE");
            Instantiate(surprisedEffect, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
            bOnce[4] = true;
        }
        // 回り始める
        if (timeCount > rotateTime && timeCount < reWalkTime)
        {
            if (!bOnce[2]) { animator.SetTrigger("Newtral"); bOnce[2] = true; }
            gameObject.transform.Rotate(new Vector3(0.0f, 4.0f, 0.0f));
        }
        // 歩き始める
        if (timeCount > reWalkTime && timeCount < stopTime)
        {
            if (!bOnce[3]) { animator.SetTrigger("Escape"); bOnce[3] = true; }
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -walkSpeed * 3.0f);
        }
        // ストップ
        if (timeCount > stopTime)
        {
            animator.SetTrigger("Newtral");
        }
    }
}
