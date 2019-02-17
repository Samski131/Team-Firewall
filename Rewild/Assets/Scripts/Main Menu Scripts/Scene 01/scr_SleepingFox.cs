using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_SleepingFox : MonoBehaviour {

	static Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponentInChildren<Animator>();
		anim.SetBool("isSleeping", true);
	}
	
	// Update is called once per frame
	void Update () 
	{	
		
		anim.SetBool("isSleeping", true);
	}
}
