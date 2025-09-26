using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Title, 
    Nekogami,
    Result,
}


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance{  get; private set; }
    public SceneName CuurentScene {  get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {

    }

    // LoadScene関数 : 引数:string(遷移したいシーン名を記入),GameState(移行する状態を記入)、戻り値:なし
    // シーン遷移時に使用する関数　※遷移処理を一つにまとめたいのでこれを使用してください
    // 使用例：GameManager.Instance.LoadScene("Title", GameState.Title)
    //public void LoadScene(SceneName scene, GameState nextState = GameState.Playing)
    //{
    //    StartCoroutine(LoadSceneRoutine(scene, nextState));
    //}
    //private IEnumerator LoadSceneRoutine(SceneName scene, GameState nextState)
    //{
    //    // シーン移行するごとにロードシーンを挿む場合はこれを使用
    //    //ChangeState(GameState.Loading);

    //    AsyncOperation async = SceneManager.LoadSceneAsync(scene.ToString());
    //    while (!async.isDone)
    //    {
    //        yield return null;
    //    }

    //    // シーン遷移終了後に状態を遷移
    //    GameManager.Instance.ChangeState(nextState);
    //}

    public void LoadScene(SceneName scene, bool useTransition = false)
    {
        if (useTransition)
            StartCoroutine(LoadSceneWithFade(scene));
        else
            Load(scene);
    }

    private void Load(SceneName scene)
    {
        SceneManager.LoadScene(scene.ToString());
        GameManager.Instance?.SetStateByScene(scene);
    }

    IEnumerator LoadSceneWithFade(SceneName scene)
    {
        // フェードアウト演出
        yield return new WaitForSeconds(0.5f);

        Load(scene);
        Debug.Log(scene);

        // フェードイン演出
        yield return null;
    }

}