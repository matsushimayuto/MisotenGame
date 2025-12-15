using UnityEngine;

public class TitleArrow : MonoBehaviour
{
    RectTransform rectTransform;    // Panel上の座標
    bool isPressed = false;         // ゲームパッド入力がされているか
    float KeepPressingTime = 0.0f;  // 一度入力してからそのまま押し続けている時間
    const float limitTime = 0.25f;  // 一度入力してから再度入力を受け付けるまでの時間

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
        { 395.0f,  248.0f },
        { 395.0f,  105.0f },
        { 462.0f, -145.0f },
        { 623.0f, -462.0f }
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
        // 入力取得
        float move = Input.GetAxis("Stick_Y") + Input.GetAxis("Cross_Y");
        if (move == 0.0f) { isPressed = false; }

        // 選択肢の変更
        if (!isPressed)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || move > 0.0f)   // 上
            {
                isPressed = true;
                choice -= 1; if (choice < Choice.Start) { choice = Choice.Exit; }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || move < 0.0f) // 下
            {
                isPressed = true;
                choice += 1; if (choice > Choice.Exit) { choice = Choice.Start; }
            }
            KeepPressingTime = 0.0f;
        }
        else
        {
            KeepPressingTime += Time.deltaTime;
        }

        // 入力制限
        if (KeepPressingTime > limitTime)
        {
            isPressed = false;
            KeepPressingTime = 0.0f;
        }

        // 矢印
        rectTransform.anchoredPosition =
            new Vector2(position[(int)choice, 0], position[((int)choice), 1]);  // 現在選択しているところに移動

        // シーン遷移
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            switch (choice)
            {
                case Choice.Start:      // 最初から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 1f);
                    AudioManager.Instance.StopBGM(1.5f);
                    break;
                case Choice.Continue:   // 途中から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 1f);
                    AudioManager.Instance.StopBGM(1.5f);
                    break;
                case Choice.Credit:     // 利用規約類
                    Debug.Log("クレジット");
                    break;
                case Choice.Exit:       // 終了
                    Debug.Log("ゲーム終了");
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;    // エディタ上
#endif
                    Application.Quit(); // アプリ上
                    break;
                case Choice.Max:
                    break;
            }
        }
    }
}
