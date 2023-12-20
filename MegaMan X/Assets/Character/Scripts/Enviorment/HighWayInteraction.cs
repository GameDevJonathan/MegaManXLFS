using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighWayInteraction : MonoBehaviour
{
    [SerializeField] private BoxCollider[] _colliders;
    [SerializeField] private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Enemy")
        {            
            _rb.useGravity = true;
        }
    }
}
