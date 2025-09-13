using UnityEngine;

public class toki_GameMNG : MonoBehaviour
{
    public bool timestop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timestop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("P"))
        {
            timestop = !timestop;
        }
    }
}
