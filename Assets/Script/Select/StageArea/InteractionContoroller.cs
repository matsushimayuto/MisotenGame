// プレイヤーからUIに

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] SelectArrow menuController;
    [SerializeField] WorldGatePromptUI worldPromptUI;
    [SerializeField] WorldSelectUI worldSelectUI;
    [SerializeField] SelectPlayer player;

    bool isInArea = false;
    bool isSelectUI = false;

    void Update()
    {
        if (!isInArea) return;

        // 表示されているキーを押したら
        //if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Specific"))
        //{
        //}

        if(isSelectUI && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButton("Specific")))
        { 
            isSelectUI = false;
            //worldPromptUI.Show(transform);
            CloseWorldSelect();
        }
    }

    // 当たり判定エリアに入ったら呼び出す関数
    public void OnEnterArea()
    {
        isInArea = true;
        //worldPromptUI.Show(transform);
        if (StageManager.Instance.IsStageUnlocked(1, 5))
        {
            player.ZeroMove();
            OpenWorldSelect();
        }
    }

    // 当たり判定エリアから出たら呼び出す関数
    public void OnExitArea()
    {
        isInArea = false;
        //worldPromptUI.Hide();
    }

    // ワールドセレクトUIを表示する
    void OpenWorldSelect()
    {
        isSelectUI = true;

        // プレイヤー操作を止める
        player.SetMoveEnabled(false);

        menuController.Show();
        worldPromptUI.Hide();
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

        // フェード兼移動処理
        StartCoroutine(Fadeing(0.5f, pos));

        // 現在のワールドを更新
        StageManager.Instance.SetCurrentWorld(worldNumber);


        for (int i = 0; i < 3; i++)
            menuController.items[i].SetSelectable(true);

        menuController.Current.SetSelectable(false);

        CloseWorldSelect();
    }

    public void CloseWorldSelect()
    {
        menuController.Hide();
        worldSelectUI.Hide();
        player.SetMoveEnabled(true);
        isSelectUI = false;
    }

    // フェード関数
    private IEnumerator Fadeing(float fadeTime, Vector3 newPos)
    {
        // フェードアウト
        UIManager.Instance.FadeOut(fadeTime);
        yield return new WaitForSeconds(fadeTime);

        // 移動処理反映
        transform.position = newPos;

        switch(StageManager.Instance.CurrentWorld)
        {
            case 1:
                AudioManager.Instance.PlayBGM("SelectBGM1", 1.5f);
                break;
            case 2:
                AudioManager.Instance.PlayBGM("SelectBGM2", 1.5f);
                break;
            case 3:
                AudioManager.Instance.PlayBGM("SelectBGM3", 1.5f);
                break;
        }

        // フェードイン
        yield return new WaitForSeconds(fadeTime);
        UIManager.Instance.FadeIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);


    }

}
