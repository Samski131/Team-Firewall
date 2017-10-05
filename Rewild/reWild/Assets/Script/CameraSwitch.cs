using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch: MonoBehaviour
{


    public Camera[] cams;
    public SkinnedMeshRenderer Human;
    public MeshRenderer Wolf;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            //If FPS Camera is on, switch to 3rd person
            if (Wolf.enabled == true)
            {
                Human.enabled = true;
                Wolf.enabled = false;

            }
            else
            {
                Wolf.enabled = true;
                Human.enabled = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //If FPS Camera is on, switch to 3rd person
            if (cams[0].enabled == true)
            {
                cams[1].enabled = true;
                cams[0].enabled = false;

            }
            //If 3rd Person Camera is on, switch to FPS
            else
            {
                cams[1].enabled = false;
                cams[0].enabled = true;

            }

        }

    }
}
