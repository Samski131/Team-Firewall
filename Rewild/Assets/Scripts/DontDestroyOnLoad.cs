﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    private DontDestroyOnLoad instance;

	// Use this for initialization
	void Start () {
        if (instance)  // creating a singleton 
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
	}
}
//Made by Panagiotis Katsiadramis 
//Modified by: 