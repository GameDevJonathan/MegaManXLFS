using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladerBehaviour : MonoBehaviour
{
    [SerializeField] private int _health, _maxHealth = 30;
    [SerializeField] private bool isAlive;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Target _target;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _rb.useGravity = false;
        _target = GetComponentInChildren<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        isAlive = _health > 0;

        if (!isAlive)
        {
            _rb.useGravity = true;
            _target.Destroyed();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Projectile" && isAlive)
        {
            switch (other.gameObject.name)
            {
                case "Normal":
                    _health--;
                    break;
                case "Medium":
                    _health -= 3;
                    break;
                case "Charged":
                    _health -= 5;
                    break;

            }
        }
    }


}
