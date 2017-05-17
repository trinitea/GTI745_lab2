using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float Speed;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }
}
