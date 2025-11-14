using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Title, 
    Select,
    Nekogami,
    Stage,
    Result,
}


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance{  get; private set; }
    public SceneName CuurentScene {  get; private set; }

    [SerializeField]
    private GameObject uiManaegrPrefab;

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

    public void LoadScene(SceneName scene, bool useTransition = false, float fadeinTime = 0.8f)
    {
        if (useTransition)
            StartCoroutine(LoadSceneWithFade(scene, fadeinTime));
        else
            Load(scene);
    }

    private void Load(SceneName scene)
    {
        SceneManager.LoadScene(scene.ToString());
        GameManager.Instance?.SetStateByScene(scene);
    }

    private IEnumerator LoadSceneWithFade(SceneName scene, float fadeInTime)
    {
        // ステート変更
        GameManager.Instance.ChangeState(GameState.Loading);


        // フェードアウト演出
        UIManager.Instance.FadeOut(0.8f);
        yield return new WaitForSeconds(0.8f);

        AsyncOperation async = SceneManager.LoadSceneAsync(scene.ToString());
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
            yield return null;

        async.allowSceneActivation = true;
        yield return null;

        // フェードイン

        yield return new WaitForSeconds(fadeInTime);
        UIManager.Instance.FadeIn(0.8f);
        yield return new WaitForSeconds(0.8f);

        // 移行先のシーンの共通処理を呼び出し
        GameManager.Instance?.SetStateByScene(scene);
    }




}