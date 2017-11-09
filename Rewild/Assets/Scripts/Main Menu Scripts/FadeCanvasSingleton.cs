using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvasSingleton : MonoBehaviour {

    private static FadeCanvasSingleton instance;

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
