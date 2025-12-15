using UnityEngine;

public class TitleArrow : MonoBehaviour
{
    RectTransform rectTransform;    // Panel上の座標

    // 選択肢の一覧
    enum Choice
    {
        Start,
        Continue,
        Credit,
        Exit,
        Max,
    }
    Choice choice;

    // 矢印の座標データ
    float[,] position = new float[4, 2] {
        { 136,   96 },
        { 136,   44 },
        { 178,  -54 },
        { 230, -162 }
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        choice = Choice.Start;  // 初期状態は初めから
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(position[0, 0], position[0, 1]);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームパッド入力取得
        float move = Input.GetAxis("Stick_Y") + Input.GetAxis("Cross_Y");

        // 矢印
        if (Input.GetKeyDown(KeyCode.UpArrow) || move > 0.0f)   // 上
        {
            choice -= 1; if (choice < Choice.Start) { choice = Choice.Exit; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || move < 0.0f) // 下
        {
            choice += 1; if (choice > Choice.Exit) { choice = Choice.Start; }
        }
        rectTransform.anchoredPosition =
            new Vector2(position[(int)choice, 0], position[((int)choice), 1]);  // 現在選択しているところに移動

        // シーン遷移
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            switch (choice)
            {
                case Choice.Start:      // 最初から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 2.0f);
                    //AudioManager.Instance.StopBGM(1f);
                    break;
                case Choice.Continue:   // 途中から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 2.0f);
                    //AudioManager.Instance.StopBGM(1f);
                    break;
                case Choice.Credit:     // 利用規約類
                    Debug.Log("クレジット");
                    break;
                case Choice.Exit:       // 終了
                    Debug.Log("ゲーム終了");
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    break;
                case Choice.Max:
                    break;
            }
        }
    }
}
