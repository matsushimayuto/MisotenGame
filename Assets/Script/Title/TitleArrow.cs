using UnityEngine;

public class TitleArrow : MonoBehaviour
{
    RectTransform rectTransform;    // Panel上の座標

    // 選択肢の一覧
    enum Choice
    {
        Start,
        Continue,
        Exit,
        Max,
    }
    Choice choice;

    // 矢印の座標データ
    const float positionX = -161.4f;    // X軸は固定
    float[] positionY = new float[] { 12.2f, -44.5f, -93.3f };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        choice = Choice.Start;  // 初期状態は初めから
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(positionX, positionY[0]);
    }

    // Update is called once per frame
    void Update()
    {
        // 矢印 ToDo:ゲームパッド対応
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            choice -= 1; if (choice < Choice.Start) { choice = Choice.Exit; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            choice += 1; if (choice > Choice.Exit) { choice = Choice.Start; }
        }
        rectTransform.anchoredPosition = new Vector2(positionX, positionY[((int)choice)]);  // 現在選択しているところに移動

        // シーン遷移
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            switch (choice)
            {
                case Choice.Start:      // 最初から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 1f);
                    AudioManager.Instance.StopBGM(1f);
                    break;
                case Choice.Continue:   // 途中から
                    SceneLoader.Instance.LoadScene(SceneName.Select, true, 1f);  // とりあえずゲームシーンへ移行
                    AudioManager.Instance.StopBGM(1f);
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
