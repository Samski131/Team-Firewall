using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
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

    scr_AmbianceControl ambianceControl;
    NavMeshAgent navMeshAgent;

    //Public variable to track the foxes movement

	public enum STATE
	{
		FLEEING,
		IDLE,
		CURIOUS,
		EATING,
        GROWL
	}

    Transform AiDestination;
    public Vector3 target;
    public bool moving = false;
	public int state = 1;
    public bool triggered;
	public int framesSinceTriggered;

	public Vector3 lastPosition;
	private SphereCollider colldier;
	private GameObject food;
    private float handMovement = 0.0f;

    public float limit = 20.0f;

    public float noticeDistance = 8.0f;
    public float grownDistance  = 5.0f;


    private GameObject player;
	public STATE currentState;

	public bool flag = true;


	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        colldier = this.GetComponent<SphereCollider>();
        ambianceControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_AmbianceControl>();
        player = GameObject.FindGameObjectWithTag("Player");
        

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

        food = GameObject.FindGameObjectWithTag("Food");

        handMovement = player.GetComponent<FirstPersonController>().handMovement;
        float distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
        if(currentState != STATE.FLEEING)
        {
            if (distanceToPlayer < noticeDistance)
            {
                currentState = STATE.GROWL;

               
            }
        }
       
        //Check the distance between the player and the fox
        //if between notice_distance & growl_distance, we in the bone zone boys

        //If in the bone zone

        // Turn the fox towards to the player and play the growl sound fx
        // if the noise fx is not playing then play it

            // if the player_distance < growl
            // Run away
        
        // check the hand movement thing and if less than the amount begin the curious mode state
        // if the hand movement is above the amount 
            //run away
        //else
            //Move slightly closer



        //MASTER STATE MACHINE
        switch (currentState)
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

            case STATE.GROWL:
                growlState();
                break;
		}
    }

	private void curiousState()
	{
		Transform target;
		target = food.transform;

        if (handMovement < limit)
        {
            //eat the food
            SetDestination(target, 1);

            if ((this.transform.position - food.transform.position).magnitude < 1.0f)
            {
                //This is for if the fox is close enought to the food
                currentState = STATE.EATING;
            }
        }
        else
        {
            //The player has moved there hand too much and the fox must not flee
            currentState = STATE.FLEEING;
        }
        //Get the location of the food
        //Check that the player isnt around
        //Move the fox slowly towards the food
    }

	private void eatingState()
	{
        //Debug.Log("This is Eating State");
        ambianceControl.increaseAmbianceState();
        DestroyObject(food);

    }

    private void growlState()
    {

        //If not barking && player does not have food
        //Bark

        float distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
        if (distanceToPlayer < grownDistance)
        {
            //Run Away
            currentState = STATE.FLEEING;
        }
        if (handMovement < limit && ((food.transform.position - this.transform.position).magnitude < noticeDistance))
        {
            currentState = STATE.CURIOUS;
        }

        if(distanceToPlayer > noticeDistance)
        {
            currentState = STATE.IDLE;
        }

    }

    private void runningState()
	{

		//Debug.Log("This is Running State");


		if(flag)
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
