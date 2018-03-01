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

    Transform AiDestination;
    public Vector3 target;
    public bool moving = false;
	public int state = 1;
    public bool triggered;
    public int framesSinceTriggered;

    public Vector3 lastPosition;

    private BoxCollider colldier;
	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        colldier = this.GetComponent<BoxCollider>();

        lastPosition = this.transform.position;

        AiDestination = destination;
        SetDestination(AiDestination, 5);
        state = 2;

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
		if (this.transform.position == lastPosition)
        {
            moving = false;
        }
		else
        {
            moving = true;
            lastPosition = this.transform.position;
            framesSinceTriggered = 0;
        }

        //Switch statement to ove the fox when triggered by player
        if (triggered && !moving)
        {

            //Move Nav onto the next waypoint
            switch(state)
            {
                case 1:
                    AiDestination = destination;
                    state = 2;
                    break;
                case 2:
                    AiDestination = destination2;
                    state = 3;
                    break;
                case 3:
                    AiDestination = destination3;
                    state = 4;
                    break;
                case 4:
                    AiDestination = destination4;
                    state = 1;
                    break;

            }

            //Set the destination and remove the flag to enter this statement
            SetDestination(AiDestination, 5);
            triggered = false;

        }
        else if(!moving && (framesSinceTriggered > 150))



        {       
            //Move to a random destinatinon

            Transform wanderDestination = AiDestination;

            Vector3 rand = UnityEngine.Random.insideUnitSphere;
            wanderDestination.position = wanderDestination.position + (rand*6);
			SetDestination(wanderDestination, 6);
            framesSinceTriggered = 0;
            
        }
        
    }
	
    private void SetDestination(Transform destination, int speed)
    {

        //Set the new destination in the NavMeshAgent
        if(destination !=null)
        {
            navMeshAgent.speed = speed;
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
			target = targetVector;
        }
    }


}
