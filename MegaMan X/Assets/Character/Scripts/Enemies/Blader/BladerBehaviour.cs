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
    [SerializeField] private Light _light;
    [SerializeField] private GameObject _muzzlePoint;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _rb.useGravity = false;
        _target = GetComponentInChildren<Target>();
        _light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
        isAlive = _health > 0;

        if (!isAlive)
        {
            StartCoroutine(DeadLight());
            _rb.useGravity = true;
            _target.Destroyed();
            _muzzlePoint.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        Debug.Log(other.transform.name);
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

    IEnumerator DeadLight()
    {
        float time = 0;

        while(_light.intensity > 0)
        {
            _light.intensity = Mathf.Lerp(_light.intensity, 0, time);
            time = Mathf.Clamp(time, 0, 1);
            time += 0.3f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


}
