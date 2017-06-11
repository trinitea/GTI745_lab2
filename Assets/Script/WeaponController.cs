using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{

    public Projectile shot;
    public Transform shotSpawn;
    public float fireRate;
    public float delay;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InvokeRepeating("Fire", delay, fireRate);
    }

    void Fire()
    {
        Instantiate(shot,
                    new Vector3(shotSpawn.transform.position.x, 0.0f, shotSpawn.transform.position.z),
                    new Quaternion(shotSpawn.transform.rotation.x, shotSpawn.transform.rotation.y, 0.0f, 1.0f)
                );

        audioSource.Play();
    }
}