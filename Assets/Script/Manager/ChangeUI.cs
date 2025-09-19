using UnityEngine;
using UnityEngine.XR;

public class ChangeUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.Instance.ChangeState(GameState.Playing);
        if (Input.GetKeyDown(KeyCode.W))
            GameManager.Instance.ChangeState(GameState.Paused);
        if (Input.GetKeyDown(KeyCode.E))
            GameManager.Instance.ChangeState(GameState.Result);
        if (Input.GetKeyDown(KeyCode.R))
            GameManager.Instance.ChangeState(GameState.GameOver);

    }
}
