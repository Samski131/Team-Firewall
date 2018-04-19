using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class scr_AmbientNoiseManager : MonoBehaviour
    {


        public GameObject AudioSourceLeft;
        public GameObject AudioSourceRight;
        public GameObject AudioSourceBehind;

        private StudioEventEmitter sourceLeft;
        private StudioEventEmitter sourceRight;
        private StudioEventEmitter sourceBehind;


        // Use this for initialization
        void Start()
        {

            sourceLeft = AudioSourceLeft.GetComponent<StudioEventEmitter>();
            sourceRight = AudioSourceRight.GetComponent<StudioEventEmitter>();
            sourceBehind = AudioSourceBehind.GetComponent<StudioEventEmitter>();
            

        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}