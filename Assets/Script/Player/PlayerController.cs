using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

public enum InputController
{
    PAD,
    ACCELEROMETER,
    VIRTUAL_JOYSTICK
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
    private Image expBar;

    private int currentExp;

    [SerializeField]
    private float maxHealth;

    private float health;

    private float spawningDuration;
    private PlayerState playerState;

    // Movement
    [SerializeField]
    private InputController inputController;

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
    private float superWeaponCost = 100.0f;

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
    [SerializeField]
    private GameObject virtualSuperWeaponButton;

    private SimpleTouchAreaButton shootAreaButton;
    private SimpleTouchAreaButton superWeaponAreaButton;


    // Use this for initialization
    void Start ()
    {
        xatstart = GameModel.accelerometerXAtStart;
        yatstart = GameModel.accelerometerYAtStart;

        shootAreaButton = virtualShootButton.GetComponent<SimpleTouchAreaButton>();
        superWeaponAreaButton = virtualSuperWeaponButton.GetComponent<SimpleTouchAreaButton>();

        myRigidbody = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        mainWeaponCanShoot = true;
        mainWeaponCooldown = 0.0f;

		SetControler(GameModel.controllerMode);
        superWeaponPowerLevel = 0.0f;
        
        currentShipIndex = 0;

        ChangePlayerState(PlayerState.Spawning); 
    }

    private void SetControler(InputController newInputControler)
    {
        inputController = newInputControler;

        switch (inputController)
        {
            case InputController.VIRTUAL_JOYSTICK:
                virtualJoystick.SetActive(true);
                virtualShootButton.SetActive(true);
                break;
            case InputController.PAD:
            case InputController.ACCELEROMETER:
                virtualJoystick.SetActive(false);
                virtualShootButton.SetActive(false);
                virtualSuperWeaponButton.SetActive(false);
                break;
            default:
                Debug.Log("Input controler is unknow : " + inputController);
                break;
        }
    }


    void FixedUpdate()
    {
        float horizontalMovement = getVerticalMovement(inputController);
        float verticalMovement = getHorizontalMovement(inputController);

		if (inputController == InputController.ACCELEROMETER){
			Debug.Log ("true");
		} else {
			Debug.Log ("false");
		}

		float speedToApply = inputController == InputController.ACCELEROMETER ? speedAccelometer * GameModel.accelerometerSensitivity : speed;
		Debug.Log ("Input selected = " + inputController + " Speed to apply = " + speedToApply);
		GetComponent<Rigidbody>().velocity = new Vector3(horizontalMovement, 0.0f, verticalMovement) * speedToApply * currentShip.speedMultiplier;

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
    private float getVerticalMovement(InputController inputControler)
    {
        switch (inputControler)
        {
            case InputController.PAD:
                return Input.GetAxis("Vertical");
            case InputController.ACCELEROMETER:
				return Input.acceleration.x - xatstart;
            case InputController.VIRTUAL_JOYSTICK:
                return CnInputManager.GetAxis("Horizontal");
            default:
                Debug.Log("Input controler is unknow : " + inputControler);
                return 0;
        }
    }

    // Get the verticcal movement value from controler
    private float getHorizontalMovement(InputController inputControler)
    {
        switch (inputControler)
        {
            case InputController.PAD:
                return Input.GetAxis("Horizontal");
            case InputController.ACCELEROMETER:
			return Input.acceleration.y -yatstart;
            case InputController.VIRTUAL_JOYSTICK:
                return CnInputManager.GetAxis("Vertical");
            default:
                Debug.Log("Input controler is unknow : " + inputControler);
                return 0;
        }
    }
    
    void Update ()
    {
        // Update main weapon cooldown
        if (!mainWeaponCanShoot)
        {
            mainWeaponCooldown -= Time.deltaTime;
            if(mainWeaponCooldown <= 0)
            {
                mainWeaponCanShoot = true;
            }
        }

        if(spawningDuration > 0)
        {
            spawningDuration -= Time.deltaTime;
            if (spawningDuration <= 0) playerState = PlayerState.Alive; 
        }

        switch(inputController)
        {
            case InputController.VIRTUAL_JOYSTICK:
                if (shootAreaButton.CanFire()) FireMain();
                if (superWeaponAreaButton.CanFire()) FireSuper();
                break;

            case InputController.PAD:
                if (Input.GetButton("Fire1")) FireMain();
                if (Input.GetButtonDown("Fire2")) FireSuper();
                break;


            case InputController.ACCELEROMETER:
                if (Input.touchCount == 1) FireMain();
                if (Input.touchCount == 2) FireSuper();
                break;

        }
        

    }

    public void receiveExp(int exp)
    {
        if (exp <= 0 || currentShipIndex == shipComponent.ships.Count - 1) return;

        currentExp += exp;

        if (currentExp >= 20)
        {
            Upgrade();

            if(currentShipIndex < shipComponent.ships.Count - 1)
            {
                currentExp = 0;
            }
        }

        expBar.fillAmount = (float) currentExp / 20;
    }

    public void receiveEnergy(int energy)
    {
        if (energy <= 0 || superWeaponPowerLevel == superWeaponPowerMax || GameModel.gameDifficulty == Difficulty.Easy) return;

        superWeaponPowerLevel = Mathf.Min(superWeaponPowerLevel + energy, superWeaponPowerMax);

        superWeaponBar.fillAmount = superWeaponPowerLevel / superWeaponPowerMax;

        if(superWeaponPowerLevel >= superWeaponCost)
        {
            superWeaponAreaButton.GetComponent<Button>().interactable = true;
        }
    }

    private void Upgrade()
    {
        if (currentShipIndex == shipComponent.ships.Count)
        {
            return;
        }

        currentShipIndex++;
        SpawnShip();
    }

    public void SpawnShip()
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

    public void Respawn()
    {
        currentShipIndex = 0;
        ChangePlayerState(PlayerState.Spawning);
    }

    private void ChangePlayerState(PlayerState state)
    {
        if (playerState == state) return;

        switch(state)
        {
            case PlayerState.Spawning:
                transform.position = new Vector3(0.0f, 0.0f, 2.0f); // put back at origin or put a player spawn object on the scene
                spawningDuration = 3.0f;

                health = maxHealth;

                GetComponent<MeshCollider>().enabled = true;

                currentExp = 0;
                expBar.fillAmount = 0.0f;

                superWeaponPowerLevel = 0;
                superWeaponBar.fillAmount = 0.0f;
                superWeaponAreaButton.GetComponent<Button>().interactable = false;

                SpawnShip();
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
    
    public void FireMain()
    {
        if(!mainWeaponCanShoot || currentShip == null)
        {
            return;
        }
        
        Vector3 hardpointPosition;

        foreach (GameObject hardpoint in currentShip.HardPoints)
        {
            Instantiate(laserProjectile, 
                    new Vector3(hardpoint.transform.position.x, 0.0f, hardpoint.transform.position.z),
                    new Quaternion(hardpoint.transform.rotation.x, hardpoint.transform.rotation.y, 0.0f, 1.0f)
                );
        }

        mainWeaponSound.Play();

        mainWeaponCanShoot = false;
        mainWeaponCooldown = timeBetweenShot;
    }

    public void FireSuper()
    {
        if (superWeaponPowerLevel < superWeaponCost)
        {
            return;
        }

        StartCoroutine(ExecuteEverithingOnScreen());

        superWeaponPowerLevel -= superWeaponCost;

        superWeapon.Fire();

        superWeaponAreaButton.GetComponent<Button>().interactable = false;
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

    enum PlayerState
    {
        Dead,
        Spawning,
        Alive
    }
}
