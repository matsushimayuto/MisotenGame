using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SelectArrow : MonoBehaviour
{
    public RectTransform cursor;
    public MenuItem[] items;

    [SerializeField]
    private Vector2 cursorOffset = new Vector2(-300f, 0f);

    int index = 0;

    public MenuItem Current => items[index];

    private void Start()
    {
        index = FindNextSelectable(index, +1);
        MoveCursor();
        Currentindex();
    }

    void MoveCursor()
    {
        cursor.anchoredPosition =
            items[index].Rect.anchoredPosition + cursorOffset;
    }

    public void Next(int dir)
    {
        int next = FindNextSelectable(index, dir);
        if (next == -1) return; // ‘S•”–³Œø

        index = next;
        MoveCursor();
    }

    int FindNextSelectable(int start, int dir)
    {
        int count = items.Length;
        int i = start;

        for (int step = 0; step < count; step++)
        {
            i = (i + dir + count) % count;
            if (items[i].IsSelectable)
                return i;
        }

        return -1; // —LŒø€–Ú‚È‚µ
    } 




    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Currentindex()
    {
        if (StageManager.Instance.CurrentWorld == index + 1)
        {
            items[index].SetSelectable(false);
        }
        else
        {
            items[index].SetSelectable(true);
        }
    }

}

