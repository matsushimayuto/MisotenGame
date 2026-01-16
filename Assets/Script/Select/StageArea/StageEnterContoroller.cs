using UnityEngine;
using System.Collections;

public class StageEnterContoroller : MonoBehaviour
{
    public SelectPlayer player;
    public SelectCamera camera;
    public float candDistance = 3f;

    private StageAreaTrigger candidate;
    private bool IsEnter = false;
    void Update()
    {
        if (candidate == null || IsEnter) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            IsEnter = true;
            // プレイヤーの操作ができないようにする
            player.SetMoveEnabled(false);
            // プレイヤーを扉の判定から少し離した位置に移動させる
            CandidateBehind();
            // プレイヤーがの背後までカメラを移動させる
            camera.EnterCamera();

            // 扉のドアの開閉処理
            candidate.DoorOpen();

            StartCoroutine(MoveForSeconds(3f));

            StartCoroutine(EnterForSeconds(2f));
            //AudioManager.Instance.PlaySE("EnterSE");
            //TryEnter();
        }


    }



    public void SetStageCandidate(StageAreaTrigger stage)
    {
        candidate = stage;
    }

    public void ClearStageCandidate(StageAreaTrigger stage)
    {
        if (candidate == stage && IsEnter == false)
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
