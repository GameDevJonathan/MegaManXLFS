using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Transform _explosionPosition;

    public void DeathEvevent()
    {
        Instantiate(_particleSystem,_explosionPosition.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
