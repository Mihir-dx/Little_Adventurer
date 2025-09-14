using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DropWeapon : MonoBehaviour
{
    public List<GameObject> Weapons;

    public void DropSword()
    {
        foreach(GameObject weapon in Weapons)
        {
            weapon.AddComponent<Rigidbody>();
            weapon.AddComponent<BoxCollider>();
            weapon.transform.parent = null; // Detach from parent
        }
    }
}
