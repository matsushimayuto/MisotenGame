using UnityEngine;

public class Result : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Decide"))
        {
            SceneLoader.Instance.LoadScene(SceneName.Title, false);
        }

    }
}
