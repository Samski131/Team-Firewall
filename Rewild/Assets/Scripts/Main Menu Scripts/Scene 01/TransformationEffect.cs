using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TransformationEffect : MonoBehaviour {

    private FirstPersonController otherScript;
    public Material Transformation;
    private bool StartTransformation;
	public float EffectStrength;
    [Range(0.00f, 0.09f)]
    public float RateOfChange;

    private void Start()
    {
		
        StartTransformation = false;
		// set the value at zero for the begining.
		otherScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

		EffectStrength= 0.0f;
		Transformation.SetFloat("_EffectStrength", EffectStrength);        
    }

    private void Update()
    {
        StartTransformation= otherScript.startTransformation;

        if (StartTransformation)
        {
			// Trigger the change in height and the enabling of the fox power/vision
			otherScript.triggerTransformation = true;
			if (EffectStrength<0.1f)
            {
				EffectStrength += RateOfChange * Time.deltaTime;
				Transformation.SetFloat("_EffectStrength", EffectStrength);
            }
        }
       
        if(!StartTransformation)
        {
			if (EffectStrength>0.0f )
            {
				EffectStrength -= RateOfChange * Time.deltaTime;

				if(EffectStrength < 0.0001f)
				{
					EffectStrength = 0.0f;
				}

				Transformation.SetFloat("_EffectStrength", EffectStrength);
			
            }
        }
          
	

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		Graphics.Blit(source, destination, Transformation);
    }



}
// Created by: Panagiotis Katsiadramis 18/02/18
// modified by: Panagiotis Katsiadramis 07/03/18