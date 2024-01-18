using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePointBehaviour : MonoBehaviour
{
    [SerializeField] Transform _muzzle;
    [SerializeField] private float _shootCooldown = 2f, _shootTime = 0f;
    [SerializeField] private bool _targetFound = false, _shooting = false;
    private Vector3 _targetPosition;
    private Coroutine ShotRoutine;
    [SerializeField] GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        _muzzle = this.gameObject.transform;
        _shootTime = _shootCooldown;

    }

    // Update is called once per frame
    void Update()
    {
        _shootTime = Mathf.Clamp(_shootTime, 0, _shootCooldown);
        if(_targetFound)
        {
            if(_shootTime > 0)
            {
                _shootTime -= Time.deltaTime;
            }
            else
            {
                if (ShotRoutine == null)
                {
                    ShotRoutine = StartCoroutine(ShootRoutine(.2f));
                }
            }
        }

    }

    IEnumerator ShootRoutine(float timeBetweenShots)
    {
        int shotCounter = 5;

        while(shotCounter > 0)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            yield return new WaitForSeconds(timeBetweenShots);
            shotCounter--;
        }
        ShotRoutine = null;
        _shootTime = _shootCooldown;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other || ShotRoutine != null) return;

        if (other.transform.tag == "Player")
        {
            transform.LookAt(other.transform, Vector3.up);
            _targetFound = true;
            _targetPosition = other.transform.position;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            _targetFound = false;
            _shootTime = _shootCooldown;
        }
    }
}
