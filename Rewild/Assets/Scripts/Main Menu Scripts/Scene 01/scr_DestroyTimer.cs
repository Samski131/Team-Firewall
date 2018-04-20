using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_DestroyTimer : MonoBehaviour {

	public float destroyTime;
	// Use this for initialization
	void Start () 
	{
		Destroy(this.gameObject, destroyTime);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
