using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private Image stepImage;
    [SerializeField] private Image textImage;

    public void Set(string message, Sprite image, Sprite textimage)
    {
        stepText.text = message;
        stepImage.sprite = image;
        textImage.sprite = textimage;

        // 画像が無いステップの場合は画像枠を非表示
        stepImage.gameObject.SetActive(image != null);
        textImage.gameObject.SetActive(textimage != null);
    }
}
