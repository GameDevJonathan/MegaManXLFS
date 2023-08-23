using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed;
    [SerializeField] private int chargeLevel;
    [SerializeField] private Collider myCollider;
    [SerializeField] Rigidbody myRigidbody;

    [SerializeField] private List<Collider> collidedWith = new List<Collider>();

    private void OnEnable()
    {
        collidedWith.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == myCollider) return;
        if (collidedWith.Contains(other)) return;

        collidedWith.Add(other);

        if(other != myCollider)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        myRigidbody.AddForce(transform.forward * forwardSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    public void ForwardSpeedChange(float speed)
    {
        forwardSpeed = speed;   
    }

}
