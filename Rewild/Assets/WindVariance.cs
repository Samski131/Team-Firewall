using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindVariance : MonoBehaviour {

    public WindZone windZone = GameObject.Find("WindZone").GetComponent<WindZone>();
    private float turbulence = 1.0f;
    private float timeFunction = 0.0f;

	// Use this for initialization
	void Start ()
    {
        windZone.windPulseMagnitude = 0.5f;
        windZone.windPulseFrequency = 0.2f;
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Goes between 0 and 1 over time
        timeFunction = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup));

        //Range between 0-5
        windZone.windMain = timeFunction / 2;
        windZone.windTurbulence = timeFunction * 5;

        Debug.Log(turbulence);
        
	}
}
