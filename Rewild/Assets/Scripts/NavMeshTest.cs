using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{

    //Fields for storing the different navPoints
    [SerializeField]
    Transform destination;
    [SerializeField]
    Transform destination2;
    [SerializeField]
	Transform destination3;
	[SerializeField]
	Transform destination4;

    NavMeshAgent navMeshAgent;
    
    //Public variable to track the foxes movement
	public Vector3 target;
    public bool moving = false;
	public int state = 1;

	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
 
		SetDestination(destination);

        //Debugging
        if (navMeshAgent == null)
        {
            Debug.Log("No Nav Mesh Agent");
        }
		
	}

    void Update()
    {
		//Check if the fox is moving fast enough
        //This is able to tell wether or not the fox is deaccelerating because it's reached its point
		if ((transform.position - target).magnitude < 1)
        {
            moving = false;
        }
		else
        {
            moving = true;
        }
        
        //If the fox has reached its destination, set a new destination
       if(!moving)
        {
            if (state == 1)
            {
				SetDestination(destination);
				state = 2;
            }
                
            else if (state == 2)
            {
				SetDestination(destination2);
				state = 3;
            }
            else if (state == 3)
            {
				SetDestination(destination3);
				state = 4;
			}
			else if (state == 4)
			{
				SetDestination(destination4);
				state = 1;
			}

            //Moving is now true, debug log the new state
            moving = true;
            Debug.Log( "Changed state to state: " + state);
        }
    }
	
    private void SetDestination(Transform destination)
    {

        //Set the new destination in the NavMeshAgent
        if(destination !=null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
			target = targetVector;
        }
    }
    

}
