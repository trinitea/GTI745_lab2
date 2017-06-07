using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float Speed;
    public Team team; // or instigator.team

    public bool pierce;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            return;
        }

        IDamagable damageReceiver = (IDamagable)other.GetComponent(typeof(IDamagable));

        if (damageReceiver != null && damageReceiver.GetTeam() != team)
        {
            damageReceiver.ReceiveDamage(1.0f);

            if (!pierce)
            {
                Destroy(gameObject);
            }
        }
    }

    //IDamageable targetHealth = (IDamageable)shootHit.collider.GetComponent(typeof(IDamageable));
}
