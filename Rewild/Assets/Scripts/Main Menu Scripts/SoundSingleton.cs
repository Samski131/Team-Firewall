using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSingleton : MonoBehaviour {

    private static SoundSingleton instance;

    // Use this for initialization
    void Start()
    {
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
