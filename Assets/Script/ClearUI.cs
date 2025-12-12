using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClearUI : MonoBehaviour
{
    [SerializeField] Image darkPanel;
    [SerializeField] Image clearImage;

    void Start()
    {
        StartCoroutine(ClearAnimation());
    }

    IEnumerator ClearAnimation()
    {
        // ˆÃ‚­‚·‚é
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 2f;
            darkPanel.color =
                new Color(0, 0, 0, Mathf.Lerp(0f, 0.6f, t));
            yield return null;
        }

        // CLEAR‰æ‘œ•\Ž¦
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 3f;
            clearImage.color =
                new Color(1, 1, 1, t);
            clearImage.transform.localScale =
                Vector3.Lerp(Vector3.one * 1.75f, Vector3.one * 0.75f, t);
            yield return null;
        }
    }
}
