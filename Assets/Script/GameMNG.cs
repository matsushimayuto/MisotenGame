using UnityEngine;
using UnityEngine.SocialPlatforms;


public class GameMNG : MonoBehaviour
{
    public bool timestop;
    public Timer timer;
    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    [SerializeField, Tooltip("時間停止(秒)")] private float stopDuration = 10.0f;
    private int a=0;
   public UI UI;


    void Start()
    {
        timestop = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonDown("TimeStop"))
        {
            timestop = !timestop;

            if (timestop)
            {
                timer.StartTimer(stopDuration, EndTimeStop);
                Debug.Log("時間止めた");
                UI.Show();
            }
            else
            {
                a = 3;
                EndTimeStop();
                
            }
            
        }

        // デバッグ用シーン遷移
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            SceneLoader.Instance.LoadScene(SceneName.Result, false);
        }

    }

    public void EndTimeStop()
    {
        // タイマーが走っていたら止める
        timer.StopTimer();
        a++;
        if (a < num)
        {
            Debug.Log("フェーズ"+a+"終了");
            foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
            {
                b.addMovenum(timestop);
            }
            timer.StartTimer(stopDuration, EndTimeStop);
        }
        else
        {
            timestop=false;
            Debug.Log("時間停止終了");

            // ブロック移動開始処理
            foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
            {
                Debug.Log("移動");
                b.ReleaseStoredForce(0);
                b.DestroyArrow();
            }
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
            //全てのブロックが止まっているので次のふぇーずへ移行
            foreach (Block b in FindObjectsOfType<Block>())
            {
                b.addMovenum(timestop);
            }
            UI.Hide();//フェーズUI（仮）減らす処理
        }
    }
}