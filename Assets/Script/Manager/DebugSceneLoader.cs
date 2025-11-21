using UnityEngine;

public class DebugSceneLoader : MonoBehaviour
{

    [SerializeField] private SceneName startscene = SceneName.Title;
    [SerializeField] private GameObject AudioManager;
    [SerializeField] private GameObject UIManager;

    private void Start()
    {
        if(Object.FindFirstObjectByType<GameManager>() == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("SceneLoader").AddComponent<SceneLoader>();
        }

        if (Object.FindFirstObjectByType<AudioManager>() == null)
        {
            Instantiate(AudioManager);
            Instantiate(UIManager);
        }

        SceneLoader.Instance.LoadScene(startscene);
    }


}
