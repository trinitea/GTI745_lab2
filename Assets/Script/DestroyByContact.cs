using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour, IDamagable {

    public int points = 1;
    public ParticleScript explosion;

	void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boundary")
        {
            return;
        }

        if (other.tag == "Player")
        {
            ( (IDamagable)other.GetComponent(typeof(IDamagable)) ).ReceiveDamage(1.0f);
        }
        else
        {
            ReceiveDamage(1.0f); // for now self inflict damage
        }
    }

    public void ReceiveDamage(float damage)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateScore(points);

        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
