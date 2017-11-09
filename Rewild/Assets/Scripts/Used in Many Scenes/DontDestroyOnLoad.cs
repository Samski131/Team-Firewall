using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    //private static DontDestroyOnLoad instance;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);

        //if (instance)  // creating a singleton 
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    instance = this;
            
        //}
    }
}
//Made by Panagiotis Katsiadramis 
//Modified by: 