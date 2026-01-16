using System.Collections;
using UnityEngine;


public class GameMNG : MonoBehaviour
{
    public enum ResultType
    {
        Clear,
        GameOver
    }

    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    [SerializeField, Tooltip("時間停止(秒)")] private float stopDuration = 10.0f;
   
    bool check=true;
    bool bFlagCollect = false;  // ゲームオーバーフラグを回収したか
    bool bGameOver = false;     // ゲームオーバーフラグ
    bool bAppearButler = false; // 執事を出すフラグ
    const float appearTime = 3.3f;  // フラグが立ってから出すまでの時間
    float timeCount = 0.0f;         // ↑のカウント用
    private Animator PlayerAnim;    // アニメーション切り替え用(プレイヤー)

    void Start()
    {
        if (GameManager.Instance.IsFirstStageEnter)
        {
            // ゲーム全体を停止
            Time.timeScale = 0.0f;

            // Start表示
            GameManager.Instance.ChangeState(GameState.Start);

            StartCoroutine(Delay(3.0f, () =>
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }));
        }
        else
        {
            // GameOver後などは即プレイ
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        PlayerAnim = GameObject.Find("Player(Clone)").GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonDown("Decide"))
        {
            // いずれかのブロックに方向指定をしているか
            bool isExistMoveBlock = false;

            Time.timeScale = 2.0f;
            
            foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
            {
                for(int i = 0; i < num; i++)
                {
                    if(b.CheckReserve(i)) { isExistMoveBlock = true; break; }   // 方向指定されている場合は処理を進める
                }
                if (isExistMoveBlock) { break; }
            }

            if (check && isExistMoveBlock)
            { // ブロック移動開始処理
                bAppearButler = true;
                foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
                {
                    b.DestroyArrow();
                    Debug.Log("移動");
                    if (b.ReleaseStoredForce(0))
                    {
                        check = false;                     
                    }
                }
                if (!check)
                {
                    foreach (Enemy e in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
                    {
                        e.StartInfiniteLook();
                    }
                }
                Check();
            }
        }

        // 執事出現
        if (bAppearButler)
        {
            timeCount += Time.deltaTime;
            if(timeCount > appearTime)
            {
                foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
                {
                    if (b.CheckReserve(b.GetPhase() + 1)) { b.AppearButler(b.GetPhase() + 1); }
                }
                timeCount = 3.1f;
                bAppearButler = false;
            }
        }

        // ポーズ画面遷移
        if(Input.GetButtonDown("Pause"))
        {
            /*ポーズ画面に関するコードを追加*/
            Debug.Log("ポーズ画面遷移");
        }

        // デバッグ用シーン遷移
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader.Instance.LoadScene(SceneName.Result, false);
        }

    }

    //全ブロック停止中か判定
    public void Check()
    {
        bool _check = true;
        foreach (Block b in FindObjectsOfType<Block>())
        {
            Debug.Log("移動");
            if (b.CheckMove())
            {
                //1つ以上動いている
                _check = false;
            }
        }

        if (_check)
        {
            // 敵がすべて倒されていたらシーン遷移
            if (Object.FindFirstObjectByType<Enemy>() == null)
            {
                StartCoroutine(ResultSequence(ResultType.Clear));
                return;
            }

            bool _bMoveReserve = false; // ゲームオーバーフラグ
            //全てのブロックが止まっているので次のふぇーずへ移行
            foreach (Block b in FindObjectsOfType<Block>())
            { 
                // フェーズ3までに倒しきれなかったらゲームオーバー
                if (b.GetPhase() == 2) { bGameOver = true; }

                // 次のフェーズで1つ以上ブロックが動くか
                if (b.CheckReserve(b.GetPhase() + 1))
                {
                    if(b.GetPhase() == 0) { bAppearButler = true; }
                    _bMoveReserve = true; // 最低でもどれか1つは動くのでゲームオーバーではない
                }

                b.addMovenum(true);
            }

            // 1つもブロックが動かないならゲームオーバー
            if (!_bMoveReserve) { bGameOver = true; }

            if(bGameOver && !bFlagCollect)
            {
                bFlagCollect = true;
                StartCoroutine(ResultSequence(ResultType.GameOver));
            }
        }
    }

    // リザルト演出
    IEnumerator ResultSequence(ResultType type)
    {
        // ゲーム進行停止
        Time.timeScale = 0.0f;

        // ===== カメラ演出 =====
        ResultCamera resultCam = FindFirstObjectByType<ResultCamera>();
        Player player = FindFirstObjectByType<Player>();

        if (resultCam != null && player != null)
        {
            resultCam.SetTarget(player.transform);
            resultCam.BeginResult();
            resultCam.MoveToPlayer();
            yield return new WaitForSecondsRealtime(1.0f);

            // プレイヤーをカメラ正面に向ける
            yield return StartCoroutine(
                RotatePlayerToCamera(player, resultCam.transform, 0.5f)
            );
        }

        // ===== プレイヤーモーション =====
        if (type == ResultType.Clear)
        {
            // クリアモーション
            PlayerAnim.SetTrigger("Success");
        }
        else
        {
            // ゲームオーバーモーション
            PlayerAnim.SetTrigger("Miss");
        }
        yield return new WaitForSecondsRealtime(2.0f);

        // ===== 状態切り替え =====
        if (type == ResultType.Clear)
        {
            GameManager.Instance.ChangeState(GameState.Result);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }

        // ===== UI 表示待ち =====
        yield return new WaitForSecondsRealtime(3.0f);

        // ゲーム再開
        Time.timeScale = 1f;

        // ===== 遷移 =====
        if (type == ResultType.Clear)
        {
            SceneLoader.Instance.LoadScene(SceneName.Select, true);
        }
        else
        {
            GameManager.Instance.IsFirstStageEnter = false;
            SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2.0f);
        }

        resultCam.EndResult();
    }

    IEnumerator RotatePlayerToCamera(Player player, Transform cam, float rotateTime)
    {
        Transform tr = player.transform;

        // カメラの前方向（水平化）
        Vector3 camForward = cam.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f)
            yield break;

        Quaternion startRot = tr.rotation;
        Quaternion targetRot = Quaternion.LookRotation(-camForward);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / rotateTime;
            tr.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        tr.rotation = targetRot;
    }

    // 遅延関数
    private IEnumerator Delay(float time, System.Action action)
    {
        yield return new WaitForSecondsRealtime(time);
        action?.Invoke();
    }

    public IEnumerator Restart()
    {
        //3秒停止する
        Time.timeScale = 0;
        yield return new WaitForSeconds(3.0f);

        //再開する
        Time.timeScale = 1;
    }
}