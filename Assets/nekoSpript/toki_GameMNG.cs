using UnityEngine;


public class toki_GameMNG : MonoBehaviour
{
    public bool timestop;
    [SerializeField, Tooltip("ブロック移動回数")] public int num=3;//ブロック移動回数
    public toki_UI UI;
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

                foreach (toki_Block b in FindObjectsOfType<toki_Block>())
                {
                    Debug.Log("移動");
                    b.ReleaseStoredForce(0);
                }
            }
            else { UI.Show(); }
            
        }
    }

    public void Check()
    {
        bool _check = true;
        foreach (toki_Block b in FindObjectsOfType<toki_Block>())
        {
            Debug.Log("移動");
            if(b.CheckMove())
            {
                _check = false;
            }
        }

        if(_check)
        {
            foreach (toki_Block b in FindObjectsOfType<toki_Block>())
            {
                b.addMovenum();
            }
            UI.Hide();
        }
    }
}
