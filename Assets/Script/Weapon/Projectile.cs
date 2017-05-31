using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float Speed;
    //public GameObject instigator;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }

    //IDamageable targetHealth = (IDamageable)shootHit.collider.GetComponent(typeof(IDamageable));
}
