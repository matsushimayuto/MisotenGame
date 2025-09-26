using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float time;
    private Action onFinished;
    private bool running;

    // タイマー開始
    public void StartTimer(float duration, Action callback)
    {
        time = duration;
        onFinished = callback;
        running = true;
    }

    void Update()
    {
        if (!running) return;

        time -= Time.deltaTime;
        if (time <= 0.0f)
        {
            running = false;
            onFinished?.Invoke();
        }
    }

    public void StopTimer()
    {
        running = false;
        onFinished = null;
    }

}
