using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SerialCommunicator)/*, typeof(Slider)*/)]
public class PossessedSlider : MonoBehaviour
{
    private SerialCommunicator serial;

	void Start()
    {
        serial = GetComponent<SerialCommunicator>();
        //serial.Open();
        InvokeRepeating("UsartValue", 1.0f, 1.0f);
        Debug.Log("Has Started");
    }

    public void UsartValue()
    {
        Debug.Log("Invoking");
        
        string bytes = serial.Read();
        if (!string.IsNullOrEmpty(bytes))
        {
            Debug.Log(bytes);
        }
        else
        {
            Debug.Log("Found nothing");
        }
    }
}
