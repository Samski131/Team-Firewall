using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentAura : MonoBehaviour {



	Quaternion fixedRotation;

	void Awake () {
		fixedRotation = transform.rotation;
	}
	

	void Update () {
		transform.rotation = fixedRotation;
	}
}
