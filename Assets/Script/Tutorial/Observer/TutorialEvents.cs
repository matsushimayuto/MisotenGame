using UnityEngine;
using System;

public class TutorialEvents : MonoBehaviour
{
    public static event Action OnCustomEvent;

    public static void Trigger()
    {
        OnCustomEvent?.Invoke();
    }
}
