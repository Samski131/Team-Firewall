using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellSource : MonoBehaviour {

	public Collider trigger;
	public ParticleSystem part;

	//This script controls a particle system that represents a scent. It toggles the particles on and off depending on whether the player is close enough and whether or not he is in fox mode

	void Start () {
		part = GetComponent<ParticleSystem> ();
		part.Stop ();
		trigger = GetComponent<SphereCollider> ();
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
			//part.startColor = new Color(1,1,1,0);
		}
	}
		
}
