using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour, IDamagable {

    public int points;

    public int energy;
    public int exp;

    public ParticleScript explosion;

	void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Boundary")
        {
            return;
        }

        IDamagable damageReceiver = (IDamagable)other.GetComponent(typeof(IDamagable));

        if (damageReceiver != null && damageReceiver.GetTeam() != Team.Enemy)
        {
            damageReceiver.ReceiveDamage(1.0f);
        }
    }

    public Team GetTeam()
    {
        return Team.Enemy;
    }

    public void ReceiveDamage(float damage)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().UpdateScore(points);

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (player)
        {
            player.receiveExp(exp);
            player.receiveEnergy(energy);
        }

        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
