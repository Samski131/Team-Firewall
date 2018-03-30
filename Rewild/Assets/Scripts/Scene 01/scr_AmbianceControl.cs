﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class scr_AmbianceControl : MonoBehaviour {

   private float[,] fogRGB = new float[5, 3] {
        { 0.06f,0.06f,0.06f},       //STATE 1 DARKNESS/BEGINNING
        { 0.12f,0.12f,0.12f},    //STATE 2 FIRST FEEDING
        { 0.18f,0.18f,0.18f},    //STATE 3 SECOND FEEDING
        { 0.24f,0.24f,0.24f},    //STATE 4 THIRD FEEDING
        { 0.24f,0.24f,0.24f}     //STATE 5 FOX
    };


    private float[,] lightRGB = new float[5, 3] {
        { 0.06f,0.06f,0.06f},       //STATE 1 DARKNESS/BEGINNING
        { 0.12f,0.12f,0.14f},    //STATE 2 FIRST FEEDING
        { 0.18f,0.18f,0.21f},    //STATE 3 SECOND FEEDING
        { 0.25f,0.25f,0.3f},    //STATE 4 THIRD FEEDING
        { 0.25f,0.25f,0.3f}     //STATE 5 FOX
    };


    private float[] fogDensity = new float[5] { 0.5f, 0.4f, 0.2f, 0.1f, 0.1f };

    private enum STATE
    {
        Isolation,
        FirstInteraction, //Scent Mechanic
        SecondInteraction, //Drop the torch
        ThirdInteraction, 
        Fox
    };

    private GlobalFog globalFogScript;
    private Light dirLight;
    public GameObject torchHand;
    private STATE state;
    private float[] curFogRGB;
    private float[] curlightRGB;
    private float curFogDensity;
    public float transitionSpeed ;//0.05
    static float t = 0.0f;
    // Use this for initialization
    void Start ()
    {
		globalFogScript = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<GlobalFog>();
        dirLight = GameObject.FindGameObjectWithTag("DirLight").GetComponent<Light>();
       // torchHand = GameObject.FindGameObjectWithTag("torch");

        //setup initial values using the isolation values
        state = STATE.Isolation;
            //get values
            curFogRGB = new float[3] { fogRGB[(int)STATE.Isolation,0],fogRGB[(int)STATE.Isolation, 1],fogRGB[(int)STATE.Isolation, 2] };
            curlightRGB = new float[3] { lightRGB[(int)STATE.Isolation, 0], lightRGB[(int)STATE.Isolation, 1], lightRGB[(int)STATE.Isolation, 2] };
            curFogDensity = fogDensity[(int)STATE.Isolation];

            //set values
            RenderSettings.fogColor = new Color(curFogRGB[0], curFogRGB[1], curFogRGB[2], 1);
            dirLight.color = new Color(curlightRGB[0], curlightRGB[1], curlightRGB[2], 1);
            globalFogScript.heightDensity = curFogDensity;

    }
	
    // Update is called once per frame
	void Update ()
    {
        
        //this is a temporary control. It should be controlled by another script calling the increaseAmbianceState() function once a "trust" value reaches a certain threshold
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Debug.Log("Increase state");
            increaseAmbianceState();
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            Debug.Log("Decrease state");
            decreaseAmbianceState();
        }


        if (state != STATE.Isolation && curFogRGB[0] != fogRGB[(int)state,0]) //don't try to interpolate if you are in isolation as you can't get values from the previous (non existant) state or you are already in position
        {
            for (int i = 0; i < 3; i++)
            {
                curFogRGB[i] = Mathf.Lerp(fogRGB[(int)state - 1, i], fogRGB[(int)state, i], t); //interpolate from the last state to the current state and set the new value as the current value for fog
                curlightRGB[i] = Mathf.Lerp(lightRGB[(int)state - 1, i], lightRGB[(int)state, i], t);
            }

            for (int i = 0; i < 5; i++)
            {
                curFogDensity = Mathf.Lerp(fogDensity[(int)state - 1], fogDensity[(int)state], t);
            }
            t += transitionSpeed * Time.deltaTime; // increase distance to interpolate

            RenderSettings.fogColor = new Color(curFogRGB[0], curFogRGB[1], curFogRGB[2], 1);
            dirLight.color = new Color(curlightRGB[0], curlightRGB[1],curlightRGB[2],1);
            globalFogScript.heightDensity = curFogDensity;
            //Debug.Log("Fog Color: " + new Color(curFogRGB[0], curFogRGB[1], curFogRGB[2], 1) + " Actual Fog Colour: " + RenderSettings.fogColor);
            //Debug.Log("Light Color: " + new Color(curlightRGB[0], curlightRGB[1], curlightRGB[2], 1) + " Actual Light Colour: " + dirLight.color);
            //Debug.Log("Fog Density: " + curFogDensity);
            //Debug.Log("STATE: " + state);
        }
	}

    public void increaseAmbianceState()
    { 
        if (state != STATE.Fox)
        {
            state++;
            t = 0;

            if (state == STATE.FirstInteraction)
            {
            }
            else if (state == STATE.SecondInteraction)
            {
                torchHand.SetActive(false);
            }
            else if (state == STATE.ThirdInteraction)
            {
            }
            else if (state == STATE.Fox)
            {
            }

        }
        else
        {
            Debug.Log("Player already in Fox state");
        }
    }

    public void decreaseAmbianceState()
    {
        if (state != STATE.Isolation)
        {
            state--;
            t = 0;
        }
        else
        {
            Debug.Log("Player already in Isolation state");
        }
    }





}
