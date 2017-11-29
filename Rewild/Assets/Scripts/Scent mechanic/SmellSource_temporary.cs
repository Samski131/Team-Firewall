using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellSource_temporary : MonoBehaviour {

	public Collider trigger;
	public ParticleSystem part;
	ParticleSystem.EmissionModule emission;
	public float decayRate = 10f; // Every second this value is deduced from the emission rate (a 100)
	float ems;

	//This script controls a particle system that represents a scent. It toggles the particles on and off depending on whether the player is close enough and whether or not he is in fox mode
	//This version is a "temporary scent", it fades away over a set time, then the object is destroyed

	void Start () {
		part = GetComponent<ParticleSystem> ();
		emission = part.emission;
		ems = emission.rate.constantMax;
		trigger = GetComponent<SphereCollider> ();
		part.Stop ();
		InvokeRepeating ("fadeScent", 0f, 1f);

	}


	void Update () {

	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Player") {
			part.Play();


		}
	}

	void OnTriggerExit(Collider col){
		if (col.tag == "Player") {
			part.Stop ();
		}
	}

	void fadeScent(){
		SetValue ();

	}



	void SetValue (){
		emission.rate = new ParticleSystem.MinMaxCurve (ems);
		ems = ems - decayRate;
		if (ems < 0) {
			Destroy (this.gameObject);
		}
	}

}
