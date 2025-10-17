using UnityEngine;
using UnityEngine.SocialPlatforms;


public class GameMNG : MonoBehaviour
{
    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    [SerializeField, Tooltip("時間停止(秒)")] private float stopDuration = 10.0f;
   
    bool check=true;

    void Start()
    { 

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonDown("Decide"))
        {
            if (check)
                // ブロック移動開始処理
                foreach (Block b in FindObjectsByType<Block>(FindObjectsSortMode.None))
                {
                    Debug.Log("移動");
                    if (b.ReleaseStoredForce(0)) 
                    {
                        check = false;
                        b.DestroyArrow();
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
            //全てのブロックが止まっているので次のふぇーずへ移行
            foreach (Block b in FindObjectsOfType<Block>())
            {
                b.addMovenum(true);
            }
        }
    }
}