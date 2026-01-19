using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageEnterContoroller : MonoBehaviour
{
    [SerializeField] StageAreaPromptUI stagePromptUI;
    [SerializeField] StageLockPromptUI lockPromptUI;
    public SelectPlayer player;
    public SelectCamera camera;
    public float candDistance = 3f;

    private StageAreaTrigger candidate;
    private bool IsEnter = false;
    void Update()
    {
        // ヌルチェック
        if (candidate == null || IsEnter) return;

        // ステージがアンロックされているかどうか
        if ((StageManager.Instance.IsStageUnlocked(candidate.GetWorldNumber(), candidate.GetStageNumber()) == true))
        {
            // ステージエリア内でキーが押されるとステージに入る処理に移行
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
            {
                // PromptUIをOffにする 
                stagePromptUI.Hide();

                // 進入フラグをON
                IsEnter = true;
                // プレイヤーの操作ができないようにする
                player.SetMoveEnabled(false);
                // プレイヤーの当たり判定Off
                player.SetPlayerIsTrigger(true);
                // プレイヤーを扉の判定から少し離した位置に移動させる
                CandidateBehind();
                // プレイヤーがの背後までカメラを移動させる
                camera.EnterCamera();

                // 扉のドアの開閉処理
                candidate.DoorOpen();

                StartCoroutine(MoveForSeconds(3f));

                StartCoroutine(EnterForSeconds(2f));
            }
        }
    }

    public void SetStageCandidate(StageAreaTrigger stage)
    {
        if (IsEnter == true) return;
        candidate = stage;

        if ((StageManager.Instance.IsStageUnlocked(stage.GetWorldNumber(), stage.GetStageNumber()) == true))
        {
            stagePromptUI.Show(transform);
        }
        else
        {
            //AudioManager.Instance.PlaySE("ロック音");
            lockPromptUI.Show(transform);
        }
    }

    public void ClearStageCandidate(StageAreaTrigger stage)
    {
        if (candidate == stage && IsEnter == false)
            candidate = null;

        if ((StageManager.Instance.IsStageUnlocked(stage.GetWorldNumber(), stage.GetStageNumber()) == true))
        {
            stagePromptUI.Hide();
        }
        else
        {
            lockPromptUI.Hide();
        }
    }

    // プレイヤーがステージに入る処理を行う
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

    // ステージごとに扉から一定の距離プレイヤーを離しておく関数
    void CandidateBehind()
    {
        Vector3 candBehind = new Vector3();

        // ステージの数字ごとで場合分けしている(書き方が微妙なのであとで変更予定)
        if(candidate.GetStageNumber() == 1 || candidate.GetStageNumber() == 3)
        {
            candBehind = candidate.transform.position - Vector3.left * candDistance;
        }
        else if(candidate.GetStageNumber() == 2 || candidate.GetStageNumber() == 4)
        {
            candBehind = candidate.transform.position - Vector3.right * candDistance;
        }
        else if(candidate.GetStageNumber() == 5)
        {
            candBehind = candidate.transform.position - Vector3.forward * candDistance;
        }

        // 扉からの一定距離にプレイヤーを移動
        player.SetPlayerPos(candBehind);
    }

    // プレイヤーを一定時間前に移動させるコルーチン
    IEnumerator MoveForSeconds(float sec)
    {
        float t = 0f;

        while (t < sec)
        {
            t += Time.deltaTime;
            transform.position += transform.forward * 1.8f * Time.deltaTime;
            yield return null;
        }
    }

    // プレイヤーがステージを読み込むまでにディレイをかけるコルーチン
    IEnumerator EnterForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);

        TryEnter();
    }

}
