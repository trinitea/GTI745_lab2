using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CnControls;

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

    private Boolean gameover = false;

    // Movement
    [SerializeField]
    private InputControler inputControler;

	// For accelerometer calibration
	private float yatstart = 0;
	private float xatstart = 0;

    [SerializeField]
    private float speed = 10;
	[SerializeField]
	private float speedAccelometer = 25;

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

    // Virtual Controler 5stick + button)
    [SerializeField]
    private GameObject virtualJoystick;
    [SerializeField]
    private GameObject virtualShootButton;


    // Use this for initialization
    void Start ()
    {
		calibAcelerometer();
        myRigidbody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        mainWeaponCanShoot = true;
        mainWeaponCooldown = 0.0f;

		SetControler(InputControler.VIRTUAL_JOYSTICK);
        superWeaponPowerLevel = 0.0f;

        currentShipIndex = 0;

        Spawn();
    }

    private void SetControler(InputControler newInputControler)
    {
        inputControler = newInputControler;

        switch (inputControler)
        {
            case InputControler.VIRTUAL_JOYSTICK:
                virtualJoystick.SetActive(true);
                virtualShootButton.SetActive(true);
                break;
            case InputControler.PAD:
            case InputControler.ACCELEROMETER:
                virtualJoystick.SetActive(false);
                virtualShootButton.SetActive(false);
                break;
            default:
                Debug.Log("Input controler is unknow : " + inputControler);
                break;
        }
    }


    void FixedUpdate()
    {
        // dont play if gameover
        if (gameover)
        {
            return;
        }

        float horizontalMovement = getVerticalMovement(inputControler);
        float verticalMovement = getHorizontalMovement(inputControler);

		if (inputControler == InputControler.ACCELEROMETER){
			Debug.Log ("true");
		} else {
			Debug.Log ("false");
		}

		float speedToApply = inputControler == InputControler.ACCELEROMETER ? speedAccelometer : speed;
		Debug.Log ("Input selected = " + inputControler + " Speed to apply = " + speedToApply);
		GetComponent<Rigidbody>().velocity = new Vector3(horizontalMovement, 0.0f, verticalMovement) * speedToApply;

        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(myRigidbody.position.x, boundary.XMin, boundary.XMax),
            0.0f,
            Mathf.Clamp(myRigidbody.position.z, boundary.ZMin, boundary.ZMax)
        );
        

        // Rolls when the ship move left or right
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

    // Get the verticcal movement value from controler
    private float getVerticalMovement(InputControler inputControler)
    {
        switch (inputControler)
        {
            case InputControler.PAD:
                return Input.GetAxis("Vertical");
            case InputControler.ACCELEROMETER:
				return Input.acceleration.x - xatstart;
            case InputControler.VIRTUAL_JOYSTICK:
                return CnInputManager.GetAxis("Horizontal");
            default:
                Debug.Log("Input controler is unknow : " + inputControler);
                return 0;
        }
    }

    // Get the verticcal movement value from controler
    private float getHorizontalMovement(InputControler inputControler)
    {
        switch (inputControler)
        {
            case InputControler.PAD:
                return Input.GetAxis("Horizontal");
            case InputControler.ACCELEROMETER:
			return Input.acceleration.y -yatstart;
            case InputControler.VIRTUAL_JOYSTICK:
                return CnInputManager.GetAxis("Vertical");
            default:
                Debug.Log("Input controler is unknow : " + inputControler);
                return 0;
        }
    }

	void calibAcelerometer(){
		xatstart = Input.acceleration.x;
		yatstart = Input.acceleration.y;
	}

    // Update is called once per frame
    void Update ()
    {
        // dont play if gameover
        if(gameover)
        {
            return;
        }

        // Update main weapon cooldown
        if (!mainWeaponCanShoot)
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

        if (Input.GetButton("Fire1") && inputControler != InputControler.VIRTUAL_JOYSTICK)
        {
            FireMain();
        }

		if (Input.GetButtonDown("Fire2") && inputControler != InputControler.VIRTUAL_JOYSTICK)
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

    public void FireMain()
    {
        Debug.Log("Fire main (mainWeaponCanShoot = " + mainWeaponCanShoot);
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

    public void ReceiveDamage(float damage)
    {
        // Explode
        // TODO: implement state for the state machine
        if(currentShip == null)
        {
            return;
        }

        Destroy(currentShip.gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
        
    }

    private void Gameover()
    {
        gameover = true;
    }

    enum InputControler
    {
        PAD,
        ACCELEROMETER,
        VIRTUAL_JOYSTICK
    };
}
