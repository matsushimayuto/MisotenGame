using UnityEngine;
using UnityEngine.UI;
public class SelectArrow : MonoBehaviour
{
    public RectTransform cursor;
    public RectTransform[] menuItems;

    [SerializeField]
    private Vector2 cursorOffset = new Vector2(-300f, 0f);

    int index = 0;

    private void Start()
    {
        MoveCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index = (index + 1) % menuItems.Length;
            MoveCursor();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            index = (index - 1 + menuItems.Length) % menuItems.Length;
            MoveCursor();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            switch(index)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }

    void MoveCursor()
    {
        RectTransform target = menuItems[index];

        cursor.anchoredPosition =
            target.anchoredPosition + cursorOffset;
    }

    void EnterWorld()
    {

    }
}

