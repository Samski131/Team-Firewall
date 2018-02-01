using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientSound : MonoBehaviour {


    public AudioMixerSnapshot insideTheForest;
    public AudioMixerSnapshot OutsideTheForest;
    public float transitionTime;

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

     void OnTriggerEnter(Collider other)
    {
        insideTheForest.TransitionTo(transitionTime);
    }

     void OnTriggerExit(Collider other)
    {
        OutsideTheForest.TransitionTo(transitionTime);   
    }
}
