using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentedObject : MonoBehaviour {

	public GameObject Scent; //put the temporary scent prefab here
	Vector3 Position;
	Vector3 newPosition;
	//This script makes the object it is attached to leave a temporary trail of scent when the object is moving

	void Start () {
		InvokeRepeating ("checkMovement", 0.5f, 1f);
		InvokeRepeating ("spawnScent", 1f, 1f);
	}
	

	void Update () {
		
	}

	void spawnScent(){
		newPosition = this.transform.position;
		if (Position != newPosition) { //if its moving do
			GameObject track = Instantiate (Scent, transform.position, transform.rotation) as GameObject;
		}

	}

	void checkMovement(){
		Position = this.transform.position;

	}
		
}
