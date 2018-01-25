﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInteraction : MonoBehaviour {
    private bool didThePlayerEnter; // did the player enter the collision area
    public Scrollbar interaction;
    private bool interactionAchieved;

	// Use this for initialization
	void Start () {
        didThePlayerEnter = false;
        //interaction.value = 0.5f;
        interactionAchieved = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (interaction.value >0.98f)// 
        {
            interactionAchieved = true;
        }
       if (interaction.value<0.1)
        {
            interactionAchieved = false;
        }

	
        if (didThePlayerEnter)
        {
            if (!interactionAchieved) // if its achieved the player doesnt need to do something else. 
            {
                interaction.value -= 0.01f; // the animal loses trust in a smaller degree when the player is close
				if (Input.GetButtonDown("Interact"))
                {
                    interaction.value += 0.15f;

                }
            }
        }
        else
        {
           interaction.value -= 0.15f;
        }
	}

    void OnTriggerEnter(Collider collider)
    {
		if (collider.gameObject.tag == "Player")
		{
        didThePlayerEnter = true;
		}
    }

     void OnTriggerExit(Collider collider)
    {
		if (collider.gameObject.tag == "Player")
		{
			didThePlayerEnter = false;
		}
    }
}
//Made by Panagiotis Katsiadramis
//Modified by: