using UnityEngine;
using UnityEngine.SceneManagement;

public class StageEnterContoroller : MonoBehaviour
{
    private StageAreaTrigger candidate;
    private bool IsEnter = false;

    void Update()
    {
        if (candidate == null || IsEnter) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            IsEnter = true;
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
        SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2.0f);
    }


}
