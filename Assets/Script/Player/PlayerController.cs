using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShipComponent
{
    public List<Ship> ships;
}

[System.Serializable]
public class Boundary
{
    public float XMin, XMax, ZMin, ZMax;
}

public enum PlayerState
{

    Dead,
    Spawning,
    Alive
}

// Mix between player pawn and player controller
public class PlayerController : MonoBehaviour, IDamagable {

    // Ship
    private Rigidbody myRigidbody;

    public Boundary boundary;
    public ShipComponent shipComponent;

    private int currentShipIndex;
    private Ship currentShip;
    private MeshFilter meshFilter;

    [SerializeField]
    private ParticleScript explosion;

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private float maxHealth;

    private float health;

    private float spawningDuration;
    private PlayerState playerState;

    // Movement
    [SerializeField]
    private float speed;

    [SerializeField]
    private float tilt;

    // Main Weapon 1
    private bool mainWeaponCanShoot = true;
    private float mainWeaponCooldown;
    
    [SerializeField]
    private float timeBetweenShot = 0.75f;

    [SerializeField]
    private Projectile laserProjectile;

    [SerializeField]
    private AudioSource mainWeaponSound; // since we have just one audio clip

    // Super Weapon
    private float superWeaponPowerLevel;
    private float superWeaponPowerMax = 100.0f;
    private float superWeaponCost = 0.0f;

    [SerializeField]
    private float superWeaponRechargeRate = 1.0f;
    [SerializeField]
    private float superWeaponRechargePerKill;
    [SerializeField]
    private SuperWeapon superWeapon;
    [SerializeField]
    private Image superWeaponBar;

    // Use this for initialization
    void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        mainWeaponCanShoot = true;
        mainWeaponCooldown = 0.0f;

        superWeaponPowerLevel = 0.0f;

        currentShipIndex = 0;

        ChangePlayerState(PlayerState.Spawning); 
    }
	
    void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        
        GetComponent<Rigidbody>().velocity = new Vector3(horizontalMovement, 0.0f, verticalMovement) * speed;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(myRigidbody.position.x, boundary.XMin, boundary.XMax),
            0.0f,
            Mathf.Clamp(myRigidbody.position.z, boundary.ZMin, boundary.ZMax)
        );
        

        // Rolls when the ship move left or right
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

	// Update is called once per frame
	void Update ()
    {
        // Update main weapon cooldown
        if(!mainWeaponCanShoot)
        {
            mainWeaponCooldown -= Time.deltaTime;
            if(mainWeaponCooldown <= 0)
            {
                mainWeaponCanShoot = true;
            }
        }

        // Update super weapon recharge
        if (superWeaponPowerLevel < superWeaponPowerMax)
        {
            superWeaponPowerLevel = Mathf.Min(superWeaponPowerLevel + superWeaponRechargeRate * Time.deltaTime, superWeaponPowerMax);
        }

        if(spawningDuration > 0)
        {
            spawningDuration -= Time.deltaTime;
            if (spawningDuration <= 0) playerState = PlayerState.Alive; 
        }

        if (Input.GetButton("Fire1"))
        {
            FireMain();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            FireSuper();
        }

        // UpdateUi
        //HealthBar.fillAmount = health / maxHealth 
        superWeaponBar.fillAmount = superWeaponPowerLevel / superWeaponPowerMax;
    }

    public void Upgrade()
    {
        if (currentShipIndex == shipComponent.ships.Count)
        {
            return;
        }

        currentShipIndex++;
        Spawn();
    }

    public void Spawn()
    {
        if(currentShip != null)
        {
            Destroy(currentShip.gameObject);
        }

        if(shipComponent.ships.Count > currentShipIndex)
        {
            currentShip = Instantiate(shipComponent.ships[currentShipIndex], transform);
        }
    }

    private void ChangePlayerState(PlayerState state)
    {
        if (playerState == state) return;

        switch(state)
        {
            case PlayerState.Spawning:
                transform.position = new Vector3(); // put back at origin or put a player spawn object on the scene
                spawningDuration = 3.0f;
                health = maxHealth;
                GetComponent<MeshCollider>().enabled = true;
                Spawn();
                break;

            case PlayerState.Alive:
                break;

            case PlayerState.Dead:
                Destroy(currentShip.gameObject);
                Instantiate(explosion, transform.position, transform.rotation);

                // Disable colision
                GetComponent<MeshCollider>().enabled = false;

                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().OnGameOver();

                break;
        }

        playerState = state;
    }

    private void FireMain()
    {
        if(!mainWeaponCanShoot || currentShip == null)
        {
            return;
        }

        Projectile projectile;
        Vector3 hardpointPosition;

        foreach (GameObject hardpoint in currentShip.HardPoints)
        {
            projectile = Instantiate(laserProjectile, 
                    new Vector3(hardpoint.transform.position.x, 0.0f, hardpoint.transform.position.z),
                    new Quaternion(hardpoint.transform.rotation.x, hardpoint.transform.rotation.y, 0.0f, 1.0f)
                );
        }

        mainWeaponSound.Play();

        mainWeaponCanShoot = false;
        mainWeaponCooldown = timeBetweenShot;
    }

    private void FireSuper()
    {
        if (superWeaponPowerLevel < superWeaponCost)
        {
            return;
        }

        StartCoroutine(ExecuteEverithingOnScreen());

        superWeaponPowerLevel -= superWeaponCost;

        superWeapon.Fire();
    }

    private IEnumerator ExecuteEverithingOnScreen()
    {
        IDamagable damagable;
        
        yield return new WaitForSeconds(0.35f);

        for (int i = 0; i < 3; i ++)
        {
            
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
            {
                damagable = (IDamagable) obj.GetComponent(typeof(IDamagable));
                if (damagable != null)
                    damagable.ReceiveDamage(3.0f);
            }

            yield return new WaitForSeconds(0.8f);
        }
    }

    public Team GetTeam()
    {
        return Team.Player; // Should not be static but ok for now
    }

    public void ReceiveDamage(float damage)
    {
        // Explode
        if(currentShip == null || playerState != PlayerState.Alive)
        {
            return;
        }

        health -= damage;
        if (health <= 0) ChangePlayerState(PlayerState.Dead);
    }
}
