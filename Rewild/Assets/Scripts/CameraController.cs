using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public bool isAnimal = true;
    public bool isTranslating = false;
    public float cameraPanSpeed;
    public GameObject animalCamAnchor;

    public GameObject Human;
    public GameObject Wolf;
 


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


      if (Input.GetKeyDown(KeyCode.V))
		{
			Debug.Log("Switching Forms (V key press)");

			if (isAnimal == true)
            { // switching to human
				isAnimal = false; // switches to human

                isTranslating = true; // starts translating
            }
			else if (isAnimal == false)
            { // switching to animal
				
				isAnimal = true; // switches to animal

                isTranslating = true; // starts translating
            }


        }


		if ((isTranslating) && (isAnimal == false)) //if the camera is translating TO animal
        {
            Debug.Log("Translating to animal anchor");
            // Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
			transform.position = Vector3.MoveTowards(transform.position, animalCamAnchor.transform.position, (cameraPanSpeed * Time.deltaTime)); //move the camera forwards smoothly based on public speed variable

			if (Vector3.Distance(transform.position, animalCamAnchor.transform.position) < 0.1)
            { //if the Third person camera is roughly where it needs to be
                isTranslating = false;
                Wolf.GetComponent<MeshRenderer>().enabled = true;
                Human.GetComponent<SkinnedMeshRenderer>().enabled = false;
                gameObject.transform.parent.transform.parent.GetComponent<CapsuleCollider>().enabled = false;
                Wolf.GetComponent<CapsuleCollider>().enabled = true;
            }

        }
		else if ((isTranslating) && (isAnimal == true)) // if camera is translating TO human
        {
            Debug.Log("Translating to human anchor");
            //Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
			transform.parent.transform.position = Vector3.MoveTowards(transform.parent.transform.position, transform.parent.position, (cameraPanSpeed * Time.deltaTime)); //move the camera forwards smoothly based on public speed variable

			if (Vector3.Distance(transform.parent.transform.position, transform.parent.position) < 0.1)
            { //if the First person camera is roughly where it needs to be
                isTranslating = false;
                Wolf.GetComponent<MeshRenderer>().enabled = false;
                Human.GetComponent<SkinnedMeshRenderer>().enabled = true;
                gameObject.transform.parent.transform.parent.GetComponent<CapsuleCollider>().enabled = true;
               Wolf.GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

}
