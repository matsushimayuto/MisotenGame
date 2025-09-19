/*
 *GameManager.cs
 *ゲームの状態・更新やシーンを管理・実行するためのスクリプト
 */
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// ゲーム状態管理用データ     ーーー他に追加したい状態があればここに記入
public enum GameState
{
    Title,
    Loading,
    Playing,
    Paused,
    Result,
    GameOver,
}


public class GameManager : MonoBehaviour
{
    [Tooltip("Singleton用の変数")]
    public static GameManager Instance { get; private set; }

    [Tooltip("現在・前の状態を取得・設定する用の変数")]
    public GameState CuurentState { get; private set; }

    [Tooltip("ゲームの状態を変更したときに実行する関数を格納するイベント用変数")]
    public event System.Action<GameState> OnStateChanged;
    //ーーーーーーーーーーーーーーーーーーーーーーーーーーー
    // 以下に関数を記述


    // Awake関数でSingletonの初期化とシーンをまたいで保存されるよう設定
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ChangeState関数 : 引数:GameState(移行させたい状態を記入)、戻り値:なし
    // 現在のゲーム状態を変えるときに使用する関数
    // ※ゲーム中にポーズ画面を出したい場合は、ChangeState(GameState.Paused)で可能！
    public void ChangeState(GameState newState)
    {
        SetState(newState); // 読み込むシーンに応じてUIをセット
        //CuurentState = newState;

        // 状態遷移時の共通処理を以下に記述
        switch (newState)
        {
            case GameState.Title:
                // AudioManager.Instance.PlayBGM("TitleBGM", 1f);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                // AudioManager.Instance.PlayBGM("StageBGM", 1.5f);
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Result:
                // リザルトUIを表示など
                break;
            case GameState.GameOver:
                // ゲームオーバー演出を再生など
                break;
        }
    }

    // LoadScene関数 : 引数:string(遷移したいシーン名を記入),GameState(移行する状態を記入)、戻り値:なし
    // シーン遷移時に使用する関数　※遷移処理を一つにまとめたいのでこれを使用してください
    // 使用例：GameManager.Instance.LoadScene("Title", GameState.Title)
    public void LoadScene(string sceneName, GameState nextState = GameState.Playing)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, nextState));
    }
    private IEnumerator LoadSceneRoutine(string sceneName, GameState nextState)
    {
        // シーン移行するごとにロードシーンを挿む場合はこれを使用
        //ChangeState(GameState.Loading);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }

        // シーン遷移終了後に状態を遷移
        ChangeState(nextState);
    }

    // SetState関数 : 引数:GameState(変更したいゲーム状態を入れる)、戻り値:なし
    // UIManagerと併用する、ゲーム状態を変更し、OnstateChangedに格納していた関数・状態を実行する関数
    // ChangedStateで呼び出すためこの関数は基本的に使用しない
    public void SetState(GameState newstate)
    {
        if (CuurentState == newstate) { return; }
        Debug.Log("SetUI");
        CuurentState = newstate;
        OnStateChanged?.Invoke(newstate);
    }


}
