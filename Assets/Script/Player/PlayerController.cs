using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


// Mix between player pawn and player controller
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed;

    [SerializeField]
    private float tilt;

    [SerializeField]
    private float timeBetweenShot = 0.75f;

    private Rigidbody myRigidbody;

    public Boundary boundary;
    public ShipComponent shipComponent;

    private int currentShipIndex;
    private Ship currentShip;
    private MeshFilter meshFilter;

    // Main Weapon 1
    private bool mainWeaponCanShoot = true;
    private float mainWeaponCooldown;

    [SerializeField]
    private Projectile laserProjectile;

    // Super Weapon
    private float superWeaponPowerLevel;
    private float superWeaponRechargeMax = 100.0f;
    private float superWeaponCost = 100.0f;

    [SerializeField]
    private float superWeaponRechargeRate = 1.0f;
    [SerializeField]
    private float superWeaponRechargePerKill;
    [SerializeField]
    private UnityEngine.Video.VideoPlayer superWeaponAnimation;

    // Use this for initialization
    void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        mainWeaponCanShoot = true;
        mainWeaponCooldown = 0.0f;

        superWeaponPowerLevel = 0.0f;

        currentShipIndex = 3;

        Spawn();
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
        if (superWeaponPowerLevel < superWeaponRechargeMax)
        {
            superWeaponPowerLevel = Mathf.Min(superWeaponPowerLevel + superWeaponRechargeRate * Time.deltaTime, superWeaponRechargeMax);
        }

        if (Input.GetButton("Fire1"))
        {
            FireMain();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            FireSuper();
        }
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
            Destroy(currentShip);
        }

        if(shipComponent.ships.Count > currentShipIndex)
        {
            currentShip = Instantiate(shipComponent.ships[currentShipIndex], transform);
        }
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

        mainWeaponCanShoot = false;
        mainWeaponCooldown = timeBetweenShot;
    }

    private void FireSuper()
    {
        
        if (superWeaponPowerLevel < superWeaponCost)
        {
            return;
        }

        superWeaponPowerLevel -= superWeaponCost;
        superWeaponAnimation.Play();
    }
}
