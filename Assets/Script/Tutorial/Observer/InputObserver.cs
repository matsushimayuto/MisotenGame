using System;
using UnityEngine;

public class InputObserver : MonoBehaviour
{
    public static event Action OnTriggerPressed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { // ”CˆÓ‚Ìƒ{ƒ^ƒ“‚É•ÏX
            OnTriggerPressed?.Invoke();
        }
    }
}
