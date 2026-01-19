using UnityEngine;
using UnityEngine.Events;

public class MenuItem : MonoBehaviour
{
    [SerializeField]
    private bool selectable = true;

    public bool IsSelectable => selectable;


    public RectTransform Rect => transform as RectTransform;

    public UnityEvent onSelected;

    public void SetSelectable(bool value)
    {
        selectable = value;

        // å©ÇΩñ⁄ÇÃîΩâfÅió·Åj
        //if (image != null)
        //    image.color = selectable ? Color.white : Color.gray;
    }

    public void Select()
    {
        if (!selectable) return;
        onSelected.Invoke();
    }
}
