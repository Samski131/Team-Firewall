using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class TunnelingControl : MonoBehaviour {

	public float maxVelocity = 6f;
	public float maxFOV = 0.7f;
	public GameObject Body;
//	public VignetteAndChromaticAberration fovLimiter;
    private CharacterController CC;
    private float speed = 0.0f;

	// Use this for initialization
	void Start () 
	{
        CC = Body.GetComponent<CharacterController>();
		//fovLimiter = this.GetComponentInParent<VignetteAndChromaticAberration>();
	}

	// Update is called once per frame
	void Update () 
	{
         speed = CC.velocity.magnitude;


		float expectedLimit = maxFOV;

		if (speed < maxVelocity)
		{
			expectedLimit = ( (speed / maxVelocity) * maxFOV);
			this.GetComponentInParent<VignetteAndChromaticAberration>().intensity = Mathf.Lerp (this.GetComponentInParent<VignetteAndChromaticAberration>().intensity, expectedLimit, 0.1f);
		}
	}
}
