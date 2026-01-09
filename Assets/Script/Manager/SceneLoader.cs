using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Title, 
    Select,
    Game,
    Stage,
    Result,
}
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private FadeController fadeController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneName scene, bool useTransition = false, float fadeTime = 0.8f)
    {
        if (useTransition)
        {
            fadeController.FadeOut(fadeTime, () =>
            {
                StartCoroutine(LoadSceneAsync(scene, fadeTime));
            });
        }
        else
        {
            Load(scene);
        }
    }

    private void Load(SceneName scene)
    {
        SceneManager.LoadScene(scene.ToString());
        GameManager.Instance?.SetStateByScene(scene);
    }

    private IEnumerator LoadSceneAsync(SceneName scene, float fadeTime)
    {
        GameManager.Instance.ChangeState(GameState.Loading);

        AsyncOperation async = SceneManager.LoadSceneAsync(scene.ToString());
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
            yield return null;

        async.allowSceneActivation = true;
        yield return null;

        GameManager.Instance?.SetStateByScene(scene);

        fadeController.FadeIn(fadeTime);
    }
}