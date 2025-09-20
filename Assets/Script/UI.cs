using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject[] UiObjects;
    private int num = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject obj in UiObjects)
        {
            obj.SetActive(false);
        }
    }
    public void Show()
    {
        foreach (GameObject obj in UiObjects) { obj.SetActive(true); }
    }

    public void Hide()
    {
        UiObjects[num].SetActive(false);
        if (num < 0) { return; }
        num --;
        
    }
}
