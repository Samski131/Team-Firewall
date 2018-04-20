using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class TunnelingControl : MonoBehaviour {

	public float maxVelocity;
	public float maxFOV = 0.7f;
	public GameObject Body;
	public GameObject CameraEye;
	public VignetteAndChromaticAberration fovLimiter;
    private CharacterController CC;
    private float speed = 0.0f;


	// Use this for initialization
	void Start () 
	{
		Body = GameObject.FindGameObjectWithTag("Player");
		CC = Body.GetComponent<CharacterController>();
		fovLimiter = CameraEye.GetComponent<VignetteAndChromaticAberration>();
	}

	// Update is called once per frame
	void Update () 
	{
         speed = CC.velocity.magnitude;


		float expectedLimit = maxFOV;

		if (speed < maxVelocity)
		{
			expectedLimit = ( (speed / maxVelocity) * maxFOV);
			this.fovLimiter.intensity = Mathf.Lerp (fovLimiter.intensity, expectedLimit, 0.1f);
		}
	}
}
