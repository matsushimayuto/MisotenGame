using UnityEngine;


public class GameMNG : MonoBehaviour
{
    public bool timestop;
    [SerializeField, Tooltip("ブロック移動回数")] public int num = 3;//ブロック移動回数
    public UI UI;

    void Start()
    {
        timestop = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            timestop = !timestop;
            Debug.Log("時間止めた");

            if (!timestop)
            {
                Debug.Log("再開");

                foreach (Block b in FindObjectsOfType<Block>())
                {
                    Debug.Log("移動");
                    b.ReleaseStoredForce(0);
                }
            }
            else { UI.Show(); }

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
                b.addMovenum();
            }
            UI.Hide();//フェーズUI（仮）減らす処理
        }
    }
}