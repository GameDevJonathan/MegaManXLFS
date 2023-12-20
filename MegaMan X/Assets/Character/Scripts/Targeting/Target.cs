using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public event Action<Target> OnDestroyed;
    public enum Type { small = 1, heavy = 2, large = 3}
    public Type type;

    public void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    public void Destroyed()
    {
        OnDestroyed?.Invoke(this);
    }



}
