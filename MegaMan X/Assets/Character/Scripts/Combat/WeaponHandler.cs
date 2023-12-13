using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;

    public void EnableWeapon()
    {
        hitBox.SetActive(true);
    }

    public void DisableWeapon()
    {
        hitBox.SetActive(false);
    }
}
