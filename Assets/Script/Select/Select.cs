using UnityEngine;

public class Select : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SceneLoader.Instance.LoadScene(SceneName.Stage, true, 2f);
    }
}
