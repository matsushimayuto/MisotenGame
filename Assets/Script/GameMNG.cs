using UnityEngine;


public class GameMNG : MonoBehaviour
{
    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    [SerializeField, Tooltip("時間停止(秒)")] private float stopDuration = 10.0f;
   
    bool check=true;
    bool bFlagCollect = false;  // ゲームオーバーフラグを回収したか
    bool bGameOver = false;     // ゲームオーバーフラグ
    void Start()
    { 

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonDown("Decide"))
        {
            if (check)
            { // ブロック移動開始処理
                foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
                {
                    Debug.Log("移動");
                    if (b.ReleaseStoredForce(0))
                    {
                        check = false;
                        b.DestroyArrow();
                    }
                }
                foreach (Enemy e in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
                {
                    e.StartInfiniteLook();
                }
            }
        }

        // デバッグ用シーン遷移
        if (Input.GetKeyDown(KeyCode.Return))
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
                SceneLoader.Instance.LoadScene(SceneName.Result, false);
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
                    b.AppearButler();
                    _bMoveReserve = true; // 最低でもどれか1つは動くのでゲームオーバーではない
                }

                b.addMovenum(true);
            }

            // 1つもブロックが動かないならゲームオーバー
            if (!_bMoveReserve) { bGameOver = true; }

            if(bGameOver && !bFlagCollect)
            {
                // GameManager.Instance.ChangeState(GameState.GameOver);
                SceneLoader.Instance.LoadScene(SceneName.Game, true, 2.0f);
                bFlagCollect = true;
            }
        }
    }
}