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

	public enum STATE
	{
		FLEEING,
		IDLE,
		CURIOUS,
		EATING
	}

    Transform AiDestination;
    public Vector3 target;
    public bool moving = false;
	public int state = 1;
    public bool triggered;
	public int framesSinceTriggered;
	public Vector3 lastPosition;
	private BoxCollider colldier;
	private GameObject food;
	public STATE currentState;

	public bool flag = true;


	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        colldier = this.GetComponent<BoxCollider>();

        lastPosition = this.transform.position;

        AiDestination = destination;
        SetDestination(AiDestination, 5);
        state = 2;

		currentState = STATE.IDLE;

        //Debugging
        if (navMeshAgent == null)
        {
            Debug.Log("No Nav Mesh Agent");
        }
		
	}

	void OnTriggerEnter(Collider other)
    {
		if(other.tag == "Player")
		{
			Debug.Log("Moving to Running State");
			currentState = STATE.FLEEING;
		}
		else if ((currentState == STATE.IDLE) && (other.tag == "Food"))
		{
			Debug.Log("Moving to CURIOUS State");
			currentState = STATE.CURIOUS;
			food = GameObject.FindGameObjectWithTag("Food");
		}

    }

	void OnTriggerExit(Collider other)
    {
        framesSinceTriggered = 0;
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


		//MASTER STATE MACHINE

		switch(currentState)
		{

			case STATE.CURIOUS:
				curiousState();
				break;

			case STATE.EATING:
				eatingState();
				break;

			case STATE.FLEEING:
				runningState();
				break;

			case STATE.IDLE:
				wanderState();
				break;
		}

        
    }

	private void curiousState()
	{
		Transform target;
		target = food.transform;

		SetDestination(target, 2);

		if(checkDestination(target.position))
			{
				//eat the food
				currentState = STATE.FLEEING;
				DestroyObject(food);
			}
		//Get the location of the food
		//Check that the player isnt around
		//Move the fox slowly towards the food
	}

	private void eatingState()
	{
		//Debug.Log("This is Eating State");

	}

	private void runningState()
	{

		//Debug.Log("This is Running State");


		if(!moving && flag)
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
			SetDestination(AiDestination, 7);
			flag = false;
		}
		else
		{
			if(checkDestination(AiDestination.position) == true)
			{
				currentState = STATE.IDLE;
				framesSinceTriggered = 0;
				flag = true;
			}
		}


	}

	private bool checkDestination(Vector3 destination)
	{
		if(Mathf.Abs(this.transform.position.magnitude - destination.magnitude) < 1.0f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private void wanderState()
	{
		//Debug.Log("This is Wander State");

		//Move to a random destinatinon
		if(!moving && (framesSinceTriggered > 150))
		{       
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
