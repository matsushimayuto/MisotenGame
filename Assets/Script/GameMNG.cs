using System.Collections;
using UnityEngine;


public class GameMNG : MonoBehaviour
{
    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    [SerializeField, Tooltip("時間停止(秒)")] private float stopDuration = 10.0f;
   
    bool check=true;
    bool bFlagCollect = false;  // ゲームオーバーフラグを回収したか
    bool bGameOver = false;     // ゲームオーバーフラグ
    bool bAppearButler = false; // 執事を出すフラグ
    const float appearTime = 3.3f;  // フラグが立ってから出すまでの時間
    float timeCount = 0.0f;         // ↑のカウント用

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
                    Debug.Log("移動");
                    if (b.ReleaseStoredForce(0))
                    {
                        check = false;
                        b.DestroyArrow();
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
                Time.timeScale = 0.0f;
                GameManager.Instance.ChangeState(GameState.Result);
                // Start表示中の遅延処理
                StartCoroutine(Delay(3.0f, () => {
                    // ゲームを再開
                    Time.timeScale = 1f;
                    // シーン遷移
                    SceneLoader.Instance.LoadScene(SceneName.Select, true);
                }));

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
                Time.timeScale = 0.0f;
                GameManager.Instance.ChangeState(GameState.GameOver);
                // Start表示中の遅延処理
                StartCoroutine(Delay(3.0f, () => {
                    // ゲームを再開
                    Time.timeScale = 1f;
                    // StartUIを非表示にする
                    GameManager.Instance.IsFirstStageEnter = false;
                    // シーン遷移
                    SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2.0f);
                }));
                bFlagCollect = true;
                // GameManager.Instance.ChangeState(GameState.GameOver);
                //SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2.0f);

            }
        }
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