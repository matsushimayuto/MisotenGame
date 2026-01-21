using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] SelectData selectData;

    // 実行中の解放状態
    private bool[,] unlockedTable;

    public int CurrentWorld { get; private set; }
    public int CurrentStage { get; private set; }

    // 初めてセレクトシーンに入ったかどうか
    public static bool isFirstSelect {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else Destroy(gameObject);
    }

    // 初期化
    void Initialize()
    {
        unlockedTable = new bool[selectData.worlds.Count, 5];

        for(int w = 0; w < selectData.worlds.Count; w++ )
        {
            for(int s = 0; s < selectData.worlds[w].stages.Count; s++)
            {
                unlockedTable[w, s] =
                    selectData.worlds[w].stages[s].isUnlocked;
            }
        }

        FirstSelect(true);
        SetCurrentWorld(1);

        if (CurrentStage == 0)
            SetCurrentStage(1);
    }

    // ステージをアンロックしているかの確認用関数
    public bool IsStageUnlocked(int world, int stage)
    {
        return unlockedTable[world - 1, stage - 1];
    }

    // ステージをアンロックする関数
    public void UnlockStage(int world, int stage)
    {
        unlockedTable[world - 1, stage - 1] = true;
    }

    // セレクトから特定のステージのcsvファイルを読み込んでシーン遷移する関数
    public void SetStage(int world, int stage)
    {
        // 入るステージの番号を上書き
        CurrentWorld = world;
        CurrentStage = stage;
        // 入るステージのcsvファイルを指定して、シーン遷移
        SceneLoader.Instance.LoadScene(SceneName.Stage, true, 1.0f);
    }

    // ワールドとステージの番号を個別設定できる関数
    public int SetCurrentWorld(int worldNumber)
    {
        CurrentWorld = worldNumber;
        return CurrentWorld;
    }
    public int SetCurrentStage(int stageNumber)
    {
        CurrentStage = stageNumber;
        return CurrentStage;
    }

    // ワールドとステージの番号を取得するための関数
    public int GetCurrentWorld()
    {
        return CurrentWorld;
    }
    public int GetCurrentStage()
    {
        return CurrentStage;
    }

    public void FirstSelect(bool enabled)
    {
        isFirstSelect = enabled;
    }

}
