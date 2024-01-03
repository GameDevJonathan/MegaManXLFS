using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighWayInteraction : MonoBehaviour
{
    [SerializeField] private BoxCollider[] _colliders;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] public bool Enemy = true;
    

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnCollisionEnter(Collision other)
    {
        


        if (other.gameObject.tag == "Enemy" && Enemy)
        {

            _rb.useGravity = true;
        }

        if (other.gameObject.tag == "Metal" )
        {
            _rb.useGravity = false;
        }
    }
}
