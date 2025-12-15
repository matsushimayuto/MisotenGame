using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] SelectData selectData;

    // Às’†‚Ì‰ğ•úó‘Ô
    private bool[,] unlockedTable;

    public int CurrentWorld { get; private set; }
    public int CurrentStage { get; private set; }


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

    }

    public bool IsStageUnlocked(int world, int stage)
    {
        return unlockedTable[world - 1, stage - 1];
    }

    public void UnlockStage(int world, int stage)
    {
        unlockedTable[world - 1, stage - 1] = true;
    }

    public void SetStage(int world, int stage)
    {
        CurrentWorld = world;
        CurrentStage = stage;
        SceneLoader.Instance.LoadScene(SceneName.Stage, true, 1.0f);

    }

    public int GetCurrentWorld()
    {
        return CurrentWorld;
    }
    public int GetCurrentStage()
    {
        return CurrentStage;
    }

}
