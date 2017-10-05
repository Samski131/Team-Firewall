using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public bool isThirdperson = true;
    public bool isTranslating = false;
    public float cameraPanSpeed;
    public GameObject FirstPersonCameraAnchor;
    private Camera FirstPersonCamera; //enables the First Person Camera.;
                                      // Use this for initialization
    public GameObject Human;
    public GameObject Wolf;
 


    void Start()
    {
        FirstPersonCamera = FirstPersonCameraAnchor.transform.Find("FPSCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C Key Press");
        }

            if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("v Key Press");
            if (isThirdperson == true)
            { // switching from third person to first person
                Debug.Log("Entering 1st Person");
                isThirdperson = false; // switches to first person

                isTranslating = true; // starts translating
            }
            else if (isThirdperson == false)
            {
                Debug.Log("Entering 3rd Person");
                isThirdperson = true; // switches to third person

                isTranslating = true; // starts translating
            }


        }


        if ((isTranslating) && (isThirdperson == false)) //if the camera is translating TO first person
        {
            Debug.Log("Translating to 1st Person");
            // Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
            transform.position = Vector3.MoveTowards(transform.position, FirstPersonCameraAnchor.transform.position, (cameraPanSpeed * Time.deltaTime)); //move the camera forwards smoothly based on public speed variable

            if (Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position) < 0.1)
            { //if the Third person camera is roughly where it needs to be
                isTranslating = false;
                transform.localPosition = new Vector3(0, 0, 0); //resets the thirdperson camera back to the anchor location for next switch.
                gameObject.GetComponent < Camera >().enabled = false; //disables Third person camera
                FirstPersonCameraAnchor.transform.Find("FPSCamera").gameObject.SetActive(true); //enables the First Person Camera.
                Wolf.GetComponent<MeshRenderer>().enabled = true;
                Human.GetComponent<SkinnedMeshRenderer>().enabled = false;
                gameObject.transform.parent.transform.parent.GetComponent<CapsuleCollider>().enabled = false;
                Wolf.GetComponent<CapsuleCollider>().enabled = true;
            }

        }
        else if ((isTranslating) && (isThirdperson == true)) // if camera is translating TO third person
        {
            Debug.Log("Translating to 3rd Person");
            //Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
            FirstPersonCamera.transform.position = Vector3.MoveTowards(FirstPersonCamera.transform.position, transform.parent.position, (cameraPanSpeed * Time.deltaTime)); //move the camera forwards smoothly based on public speed variable

            if (Vector3.Distance(FirstPersonCamera.transform.position, transform.parent.position) < 0.1)
            { //if the First person camera is roughly where it needs to be
                isTranslating = false;
                FirstPersonCamera.transform.gameObject.SetActive(false); //disables the first person camera.
                gameObject.GetComponent<Camera>().enabled = true; //disables Third person camera
                FirstPersonCamera.transform.localPosition = new Vector3(0, 0, 0);
                Wolf.GetComponent<MeshRenderer>().enabled = false;
                Human.GetComponent<SkinnedMeshRenderer>().enabled = true;
                gameObject.transform.parent.transform.parent.GetComponent<CapsuleCollider>().enabled = true;
               Wolf.GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

}
