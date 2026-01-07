using UnityEngine;
using UnityEngine.UI;
public class WorldSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class WorldButton
    {
        public int worldNumber;
        public Button button;
    }

    [SerializeField] WorldButton[] worldButtons;

    InteractionController controller;

    public void Show(InteractionController c, int currentWorld)
    {
        controller = c;
        UpdateButtons(currentWorld);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void UpdateButtons(int currentWorld)
    {
        foreach (var wb in worldButtons)
        {
            // 現在いるワールドは選択不可
            wb.button.interactable = (wb.worldNumber != currentWorld);
        }
    }

    // Button OnClick 用
    public void SelectWorld(int worldNumber)
    {
        controller.OnWorldSelected(worldNumber);
    }

    public void Cancel()
    {
        controller.CancelWorldSelect();
    }
}
