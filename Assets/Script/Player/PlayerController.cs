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

public class PlayerController : MonoBehaviour {

    public float speed;
    public float tilt;
    
    private Rigidbody myRigidbody;

    public Boundary boundary;
    public ShipComponent shipComponent;

    private Ship currentShip;

	// Use this for initialization
	void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();

	}
	
    void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        
        GetComponent<Rigidbody>().velocity = new Vector3(horizontalMovement, 0.0f, verticalMovement) * speed;

        // To be honest, I prefer use a box that trigger an event on player leave !!

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
	void Update () {
		
	}
}
