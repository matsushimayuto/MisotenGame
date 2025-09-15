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
        if (Input.GetKeyUp(KeyCode.T))
        {
            timestop = !timestop;
            Debug.Log("時間止めた");
            
            if (!timestop)
            {
                Debug.Log("再開");
                // 時間が再開したので、全ブロックに「ReleaseStoredForce」させる
                foreach (toki_Block b in FindObjectsOfType<toki_Block>())
                {
                    Debug.Log("移動");
                    b.ReleaseStoredForce();
                }
            }
            
        }
    }
}
