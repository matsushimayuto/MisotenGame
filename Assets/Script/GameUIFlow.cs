using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIFlow : MonoBehaviour
{
    [SerializeField] Image darkPanel;
    [SerializeField] Image UIimage;

    [Header("Wavy Material")]
    [SerializeField] Material wavyMaterial;

    [Header("Time Settings")]
    [SerializeField] float uiFadeInTime = 1.0f;
    [SerializeField] float wavyDuration = 3.0f;
    [SerializeField] float uiFadeOutTime = 1.0f;

    Material runtimeWavyMat;

    void Start()
    {
        runtimeWavyMat = Instantiate(wavyMaterial);
        UIimage.material = runtimeWavyMat;
        StartCoroutine(UIAnimation());
    }

    void Update()
    {
        if (runtimeWavyMat != null)
        {
            runtimeWavyMat.SetFloat("_TimeOffset", Time.unscaledTime);
        }
    }

    IEnumerator UIAnimation()
    {
        // ---------- フェードイン（ゆらゆらON） ----------
        //UIimage.material = wavyMaterial;
        UIimage.color = new Color(1, 1, 1, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / uiFadeInTime;
            UIimage.color = new Color(1, 1, 1, t);
            yield return null;
        }

        // ---------- ゆらゆらキープ ----------
        yield return new WaitForSecondsRealtime(wavyDuration);

        // ---------- フェードアウト ----------
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / uiFadeOutTime;
            UIimage.color = new Color(1, 1, 1, 1f - t);
            yield return null;
        }

        // ---------- 完全に消す ----------
        UIimage.material = null;
        UIimage.color = new Color(1, 1, 1, 0);
    }
}
