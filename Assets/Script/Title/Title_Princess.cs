using UnityEngine;

public class Title_Princess : MonoBehaviour
{
    private Animator animator;  // アニメーション
    private float timeCount = 0.0f;     // 時間のカウント
    const float reactionTime = 4.5f;    // 後ろを通り過ぎる時間
    const float settingTime = reactionTime + 1.1f;  // 泥棒の前にセッティングする時間
    private bool bOnce = false;

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
        // 泥棒の後ろ通過
        if (timeCount > reactionTime && timeCount < settingTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            gameObject.transform.position += new Vector3(0.15f, 0.0f, 0.0f);
        }
        // 泥棒の前にセッティング
        if (timeCount > settingTime && !bOnce)
        {
            // Todo:泥棒の前に出す→追いかける positionとRotateの変更
            //gameObject.transform.position = new Vector3()
            bOnce = true;
        }
    }
}
