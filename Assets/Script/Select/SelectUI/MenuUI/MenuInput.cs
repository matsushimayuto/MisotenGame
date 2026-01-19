using UnityEngine;

public class MenuInput : MonoBehaviour
{
    public SelectArrow selector;

    void Update()
    {
        // ここで矢印の選択する対象を変更している
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selector.Next(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selector.Next(+1);
        }

        // 選択した対象の処理を実行
        if (Input.GetKeyDown(KeyCode.Return))
        {
            selector.Current.Select();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cansel();
        }
    }

    void Cansel()
    {
        Debug.Log("Cansel");
    }

}
