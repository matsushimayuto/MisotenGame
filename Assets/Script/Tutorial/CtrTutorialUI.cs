using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CtrTutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image illustration;

    public void ShowStep(TutorialObject step)
    {
        text.text = step.message;
        illustration.sprite = step.illustration;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
