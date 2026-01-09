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
    private bool bOnce = false;
    private Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();    // アニメーション取得
        startPosition = gameObject.transform.position;
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
        if (timeCount > settingTime && !bOnce)
        {
            animator.SetInteger("n_MoveNum", 1);
            gameObject.transform.position = position;
            gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            bOnce = true;
        }
        // 泥棒を追いかける
        if (timeCount > chaseTime && timeCount < stopTime)
        {
            animator.SetInteger("n_MoveNum", 0);
            animator.speed = 1.5f;
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -0.08f);
        }
        // ストップ
        if (timeCount > stopTime)
        {
            animator.speed = 0.0f;

            GameManager.Instance.ChangeState(GameState.Title);
        }
    }
}
