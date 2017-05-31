using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Destroy(gameObject);
        }

        Destroy(gameObject, ps.duration);
	}
}
