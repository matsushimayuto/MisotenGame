using UnityEngine;
using UnityEngine.SceneManagement;

public class StageEnterContoroller : MonoBehaviour
{
    private StageAreaTrigger candidate;

    void Update()
    {
        if (candidate == null) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            AudioManager.Instance.PlaySE("EnterSE");
            TryEnter();
        }
    }

    public void SetStageCandidate(StageAreaTrigger stage)
    {
        candidate = stage;
    }

    public void ClearStageCandidate(StageAreaTrigger stage)
    {
        if (candidate == stage)
            candidate = null;
    }

    void TryEnter()
    {
        int w = candidate.worldNumber;
        int s = candidate.stageNumber;

        if (!StageManager.Instance.IsStageUnlocked(w, s))
            return;

        GameManager.Instance.IsFirstStageEnter = true;
        StageManager.Instance.SetStage(w, s);
        SceneLoader.Instance.LoadScene(SceneName.Stage, true, 1.0f);
    }


}
