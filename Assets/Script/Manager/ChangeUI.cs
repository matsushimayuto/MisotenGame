using UnityEngine;
using UnityEngine.XR;

public class ChangeUI : MonoBehaviour
{
    public SaveLoad saveload;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.ChangeState(GameState.Title);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.Instance.ChangeState(GameState.Playing);
        if (Input.GetKeyDown(KeyCode.W))
            GameManager.Instance.ChangeState(GameState.Title);
        if (Input.GetKeyDown(KeyCode.E))
            GameManager.Instance.ChangeState(GameState.Result);
        if (Input.GetKeyDown(KeyCode.R))
            GameManager.Instance.ChangeState(GameState.GameOver);

        //if(Input.GetKeyDown(KeyCode.S))
        //    saveload.Save();
        //if (Input.GetKeyDown(KeyCode.L))
        //    saveload.DataLoad();
    }
}
