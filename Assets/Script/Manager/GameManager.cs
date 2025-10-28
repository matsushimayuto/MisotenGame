/*
 *GameManager.cs
 *ゲームの状態・更新を管理・実行するためのスクリプト
 */
using UnityEngine;

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
    public GameState CurrentState { get; private set; }

    [Tooltip("ゲームの状態を変更したときに実行するイベントを格納する変数")]
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

    private void Start()
    {

    }

    // ChangeState関数 : 引数:GameState(移行させたい状態を記入)、戻り値:なし
    // 現在のゲーム状態を変えるときに使用する関数
    // ※ゲーム中にポーズ画面を出したい場合は、ChangeState(GameState.Paused)で可能！
    // 基本的にはUIの表示切替等で扱う予定
    public void ChangeState(GameState newState)
    {
        SetState(newState); // 読み込むシーンに応じてUIをセット

        //Debug.Log(newState);
        // 状態遷移時の共通処理を以下に記述
        switch (newState)
        {
            case GameState.Title:
                AudioManager.Instance.PlayBGM("TitleBGM", 1f);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
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

    // SetStateByScene関数 : 引数:SceneName(呼び出したいシーン名)、戻り値:なし
    // SceneLoaderからの呼び出しに応じて、対応したシーンに遷移後にゲーム状態をシーンに合わせる
    // 特定のシーンで遷移後に行いたい処理をここに追加することも可能
    // この関数はSceneLoadeで扱うので、基本的には他スクリプトで使用しない
    public void SetStateByScene(SceneName newScene)
    {
        switch(newScene)
        {
            case SceneName.Title:
                ChangeState(GameState.Title);
                break;
            case SceneName.Nekogami:
                ChangeState(GameState.Playing);
                AudioManager.Instance.PlayBGM("StageBGM", 1.5f);
                break;
            case SceneName.Result:
                ChangeState(GameState.Result);
                break;
        }
    }

    // SetState関数 : 引数:GameState(変更したいゲーム状態を入れる)、戻り値:なし
    // UIManagerと併用する、ゲーム状態を変更し、OnstateChangedに格納していた関数・状態を実行する関数
    // ChangedStateで呼び出すためこの関数は基本的に使用しない
    public void SetState(GameState newstate)
    {
        if (CurrentState == newstate) { return; }
        
        CurrentState = newstate;
        OnStateChanged?.Invoke(newstate);
    }

}
