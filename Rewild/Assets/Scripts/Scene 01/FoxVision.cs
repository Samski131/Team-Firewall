using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class FoxVision : MonoBehaviour {

    public Material foxVision;
    [Range(0,10)]
    public int iterations; // defines the blur level
    [Range(0, 4)]
    public int decreasedResolution;

    private int savedIterations;
    private int savedDecreasedResolution;
    private FirstPersonController otherScript;
    private bool Vision;

    private void Start()
    {
        //  initial values 
        savedIterations = 0;
        savedDecreasedResolution = 0;
        Vision = false;
    }




    private void Update()
    {

        otherScript= transform.parent.parent.gameObject.GetComponent<FirstPersonController>();
        Vision = otherScript.foxVision;




        if (Vision)
        {
            savedIterations = (int) Mathf.Lerp(savedIterations, iterations, 1);
            savedDecreasedResolution = (int)Mathf.Lerp(savedDecreasedResolution, decreasedResolution, 1);
            Debug.Log("animal is true");
        }
        else
        {            
            savedIterations = (int)Mathf.InverseLerp(savedIterations, 0, 1);
            savedDecreasedResolution = (int)Mathf.InverseLerp(savedDecreasedResolution, 0, 1);
          //  Debug.Log("iterations"+ iterations);
        }
    }


    private   void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // make bitwise operations and decrease the resolution of the screen
        int width = source.width >> savedDecreasedResolution;
        int height = source.height >> savedDecreasedResolution;

        RenderTexture temporary = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, temporary);

        for (int i = 0; i < savedIterations; i++)
        {
            RenderTexture temporary2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(temporary, temporary2, foxVision);
            RenderTexture.ReleaseTemporary(temporary);
            temporary = temporary2;
        }
        //source is the full rendered scene that you would normally send ot the monitor.
        Graphics.Blit(temporary, destination);
        RenderTexture.ReleaseTemporary(temporary);
    }
}

// Created by: Panagiotis Katsiadramis 13/02/18
// modified by: Panagiotis Katsiadramis 19/02/18
// modified by: Panagiotis Katsiadramis 07/03/18