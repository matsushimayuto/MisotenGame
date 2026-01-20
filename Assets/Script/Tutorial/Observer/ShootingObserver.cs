using UnityEngine;
using System;

public class ShootingObserver : MonoBehaviour
{
    public static event Action OnHit;

    public static void Hit()
    {
        OnHit?.Invoke();
    }
}
