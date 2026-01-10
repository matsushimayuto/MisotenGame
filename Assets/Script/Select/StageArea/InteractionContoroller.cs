using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] WorldGatePromptUI promptUI;
    [SerializeField] WorldSelectUI worldSelectUI;
    [SerializeField] SelectPlayer player;

    bool isInArea = false;
    bool isSelectUI = false;
    void Update()
    {
        if (!isInArea) return;

        // 表示されているキーを押したら
        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenWorldSelect();
        }

        if(isSelectUI && Input.GetKeyDown(KeyCode.Escape))
        {
            CancelWorldSelect();
            isSelectUI = false;
            promptUI.Show(transform);
        }
    }

    public void OnEnterArea()
    {
        isInArea = true;
        promptUI.Show(transform);
    }

    public void OnExitArea()
    {
        isInArea = false;
        promptUI.Hide();
    }

    void OpenWorldSelect()
    {
        isSelectUI = true;

        // プレイヤー操作を止める
        player.SetMoveEnabled(false);

        promptUI.Hide();
        int currentWorldNumber = StageManager.Instance.GetCurrentWorld();
        Debug.Log(currentWorldNumber);
        worldSelectUI.Show(this, currentWorldNumber);
    }

    // WorldSelectUI から呼ばれる
    public void OnWorldSelected(int worldNumber)
    {
        Debug.Log($"World {worldNumber} selected");

        // 現在位置を取得
        Vector3 pos = transform.position;

        // Y座標を worldNumber * 13 だけ上げる
        pos.y = (worldNumber) * 13f;


        // -----------------------------
        // ここにワールド移動処理を書く
        // （Scene遷移なし・Prefab切替など）
        // -----------------------------

        // フェード兼移動処理
        StartCoroutine(Fadeing(0.5f, pos));

        // 現在のワールドを更新
        StageManager.Instance.SetCurrentWorld(worldNumber);

        CloseWorldSelect();
    }

    public void CancelWorldSelect()
    {
        CloseWorldSelect();
    }

    void CloseWorldSelect()
    {
        worldSelectUI.Hide();
        player.SetMoveEnabled(true);
        isSelectUI = false;
    }

    private IEnumerator Fadeing(float fadeTime, Vector3 newPos)
    {
        // フェードアウト
        UIManager.Instance.FadeOut(fadeTime);
        yield return new WaitForSeconds(fadeTime);

        // 移動処理反映
        transform.position = newPos;

        // フェードイン
        yield return new WaitForSeconds(fadeTime);
        UIManager.Instance.FadeIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);


    }

}
