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
    public bool triggered;
    public int framesSinceTriggered;

    private BoxCollider colldier;
	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        colldier = this.GetComponent<BoxCollider>();


        SetDestination(destination);

        //Debugging
        if (navMeshAgent == null)
        {
            Debug.Log("No Nav Mesh Agent");
        }
		
	}

    void OnTriggerEnter()
    {
        triggered = true;
    }

    void OnTriggerExit()
    {
        framesSinceTriggered = 0;
        triggered = false;
    }

    void Update()
    {
        framesSinceTriggered++;
		//Check if the fox is currently moving around
		if ((transform.position - target).magnitude < 1)
        {
            moving = false;
        }
		else
        {
            moving = true;
        }

       //Switch statement to ove the fox when triggered by player
       if (triggered && !moving)
        {
            switch(state)
            {
                case 1:
                    SetDestination(destination);
                    state = 2;
                    break;
                case 2:
                    SetDestination(destination2);
                    state = 3;
                    break;
                case 3:
                    SetDestination(destination3);
                    state = 4;
                    break;
                case 4:
                    SetDestination(destination4);
                    state = 1;
                    break;
            }

            //Moving is now true, debug log the new state
            triggered = false;
        }
        else if(!moving && (framesSinceTriggered > 300))
        {       
            //Move to a random destinatinon

            //Transform wanderDestination;
            //wanderDestination = target;
            //Vector3 rand = UnityEngine.Random.insideUnitSphere;
            //wanderDestination.position = target.position + rand;
            //SetWanderDestination(wanderDestination);

            //Debug.Log("Should be wandering aboot");

        }

        if (framesSinceTriggered % 100 == 0)
        {
            Debug.Log(framesSinceTriggered);
        }
    


    }
	
    private void SetDestination(Transform destination)
    {

        //Set the new destination in the NavMeshAgent
        if(destination !=null)
        {
            navMeshAgent.speed = 7;
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
			target = targetVector;
        }
    }
    private void SetWanderDestination(Transform destination)
    {

        //Set the new destination in the NavMeshAgent
        if (destination != null)
        {
            navMeshAgent.speed = 2;
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
            target = targetVector;
        }
    }


}
