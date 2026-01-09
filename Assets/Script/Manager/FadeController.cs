using UnityEngine;
using System;
using System.Collections;
public class FadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup sceneFadePanel;
    private Coroutine runningFade;

    public void FadeOut(float duration, Action onComplete = null)
    {
        StartFade(1f, duration, onComplete);
    }

    public void FadeIn(float duration, Action onComplete = null)
    {
        StartFade(0f, duration, onComplete);
    }

    private void StartFade(float targetAlpha, float duration, Action onComplete)
    {
        if (runningFade != null)
            StopCoroutine(runningFade);

        runningFade = StartCoroutine(FadeRoutine(targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeRoutine(float target, float duration, Action onComplete)
    {
        float start = sceneFadePanel.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            sceneFadePanel.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        sceneFadePanel.alpha = target;
        onComplete?.Invoke();
    }
}
