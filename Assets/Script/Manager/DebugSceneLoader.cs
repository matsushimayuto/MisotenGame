using UnityEngine;

public class DebugSceneLoader : MonoBehaviour
{

    [SerializeField] private SceneName startscene = SceneName.Title;

    private void Start()
    {
        if(Object.FindFirstObjectByType<GameManager>() == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("SceneLoader").AddComponent<SceneLoader>();
        }

        SceneLoader.Instance.LoadScene(startscene);
    }


}
