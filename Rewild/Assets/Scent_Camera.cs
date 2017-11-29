using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent_Camera : MonoBehaviour {

	Camera scentCam;
	//This script enables the second (scent) camera
	void Start () {
		scentCam = GetComponent<Camera> ();
	}
	

	void Update () {
		
	}
}
