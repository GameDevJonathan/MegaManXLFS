using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public event Action<Target> OnDestroyed;

    public void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

}
