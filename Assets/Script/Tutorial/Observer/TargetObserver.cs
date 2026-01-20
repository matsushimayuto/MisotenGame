using UnityEngine;
using System;

public class TargetObserver : MonoBehaviour
{
    public static event Action OnReached;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnReached?.Invoke();
        }
    }
}
