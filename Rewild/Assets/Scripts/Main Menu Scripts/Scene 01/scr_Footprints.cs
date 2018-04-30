using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class scr_Footprints : MonoBehaviour {

    public bool leavePrints;
    public bool isFox; //for determining which kind of prints to leave as Mollie changes hers.
    public GameObject foxPrints;
    public GameObject bootPrints;
    private Vector2 lastFootprint;
    private Vector2 currentPosition;
    private Terrain terrain;
    public Queue<GameObject> printQueue;
    public Color trailColor;
	private FirstPersonController fpsScript;
    public float StrideDistance; //how far between each step
    public int TrailLength; // how long a trail can be before steps begin getting deleted
    // Use this for initialization
	void Start () 
    {
      terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
      lastFootprint = new Vector2(this.transform.position.x, this.transform.position.z); // just to get a 2d representation
      printQueue = new Queue<GameObject>();
      TrailLength++; // as counting from 0 makes the length slightly shorter than expected.
	  fpsScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
	}
	

    float distanceMoved ()
    {
        return (Vector2.Distance(lastFootprint, currentPosition));        
    }

	// Update is called once per frame
	void Update () 
    {

		//Set the footsteps to match the current state of the player (Ignores if this is the fox)
		if(tag == "Player")
		{
			isFox = fpsScript.isAnimal;
		}


        currentPosition = new Vector2(this.transform.position.x, this.transform.position.z); //we don't care about Y values when working out distance
        if (distanceMoved() >= StrideDistance)
            {
            
            if (isFox)
            {
                GameObject temp = GameObject.Instantiate(foxPrints, new Vector3(currentPosition.x, terrain.SampleHeight(this.transform.position)+ 0.01f, currentPosition.y), this.transform.localRotation);
                temp.GetComponent<Renderer>().material.SetColor("_EmissionColor", trailColor);
                RaycastHit hit;
                lastFootprint.Set(currentPosition.x, currentPosition.y);
                if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
                {
                    temp.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * temp.transform.rotation;
                    printQueue.Enqueue(temp); //current pos.y is actually Z position
                }   
            }
            else
            {
                GameObject temp = GameObject.Instantiate(bootPrints, new Vector3(currentPosition.x, terrain.SampleHeight(this.transform.position) + 0.01f, currentPosition.y), this.transform.localRotation);
                temp.GetComponent<Renderer>().material.SetColor("_EmissionColor", trailColor);
                RaycastHit hit;
                lastFootprint.Set(currentPosition.x, currentPosition.y);
                if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
                {
                    temp.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * temp.transform.rotation;
                    printQueue.Enqueue(temp); //current pos.y is actually Z position
                }   
            }
        }
            
        if (printQueue.Count >= TrailLength)
        {
            GameObject.Destroy(printQueue.Peek());
            printQueue.Dequeue();
        }
    }
}
        		
	

