using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TransformationEffect : MonoBehaviour {

    private FirstPersonController otherScript;
    public Material Transformation;
    private bool StartTransformation;
    private float EffectStrenght;
    [Range(0.00f, 0.09f)]
    public float RateOfChange;

    private void Start()
    {
        StartTransformation = false;
        // set the value at zero for the begining.
        EffectStrenght = 0.0f;
        Transformation.SetFloat("_EffectStrength", EffectStrenght);        
    }

    private void Update()
    {
        otherScript = transform.parent.parent.gameObject.GetComponent<FirstPersonController>();
        StartTransformation= otherScript.startTransformation;

        if (StartTransformation)
        {
           
            if (EffectStrenght<0.1f)
            {
                EffectStrenght += RateOfChange * Time.deltaTime;
                Transformation.SetFloat("_EffectStrength", EffectStrenght);
            }
        }
       
        if(!StartTransformation)
        {
            if (EffectStrenght>0.0f )
            {
                EffectStrenght -= RateOfChange * Time.deltaTime;
                Transformation.SetFloat("_EffectStrength", EffectStrenght);
            }
        }
          

        

    


    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
                        Graphics.Blit(source, destination, Transformation);
    }



}
// Created by: Panagiotis Katsiadramis 18/02/18