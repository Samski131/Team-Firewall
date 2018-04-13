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
	AudioSource audioSource;
    static Animator anim;
    //Public variable to track the foxes movement

	public enum STATE
	{
		FLEEING,
		IDLE,
		CURIOUS,
		EATING,
        GROWL,
        WANDER
	}

    Transform AiDestination;
	public Vector3 target;
	private bool moving = false;
	private int state = 1;
	private bool triggered;
	private int framesSinceTriggered;

	public float DistanceToDestination;

	public Vector3 lastPosition;
	private SphereCollider colldier;
	public GameObject food;
    private float handMovement = 0.0f;

    public float limit = 20.0f;

	private float noticeDistance = 8.0f;
	private float growlDistance  = 5.0f;


    private GameObject player;
	public STATE currentState;

	public bool flag = true;
	public AudioClip growlSound; //barking for the moment.
    private float eatTime;

	public float distanceToPlayer;
	public float distanceToFood;

	void Start ()
    {
        //Initiliase the variables
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        colldier = this.GetComponent<SphereCollider>();
        ambianceControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_AmbianceControl>();
        player = GameObject.FindGameObjectWithTag("Player");
		audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        food = null;

		distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
		if(food != null)
		{
			distanceToFood = (this.transform.position - food.transform.position).magnitude;
		}
		else
		{
			distanceToFood = 9999990.0f;
		}

        lastPosition = this.transform.position;

        AiDestination = destination;
        SetDestination(AiDestination.position, 5);
        state = 2;
        

		currentState = STATE.FLEEING;

        //Debugging
        if (navMeshAgent == null)
        {
            Debug.Log("No Nav Mesh Agent");
        }
		
	}
    

    void Update()
    {
        framesSinceTriggered++;
        //anim.SetInteger("State", (int)currentState);
		//update the animator
		food = GameObject.FindGameObjectWithTag("Food");

		distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
		if(food != null)
		{
			distanceToFood = (this.transform.position - food.transform.position).magnitude;
		}
		else
		{
			distanceToFood = 9999990.0f;
		}
       

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

        if (moving)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

		//MASTER STATE MACHINE
		switch (currentState)
		{

		case STATE.CURIOUS:
			Debug.Log("STATE MACHINE = CURIOUS");
			curiousState();
			break;

		case STATE.EATING:
			Debug.Log("STATE MACHINE = EATING");
			eatingState();
			break;

		case STATE.FLEEING:
			Debug.Log("STATE MACHINE = FLEEING");
			runningState();
			break;

		case STATE.IDLE:
			Debug.Log("STATE MACHINE = IDLE");
			idleState();
			break;

		case STATE.GROWL:
			Debug.Log("STATE MACHINE = GROWL");
			growlState();
			break;

		case STATE.WANDER:
			Debug.Log("STATE MACHINE = WANDER");
			wanderState();
			break;
		}

    }

	private void curiousState()
	{
		Transform target;
		if(food != null)
		{
			target = food.transform;

			//eat the food
			NavMeshHit hit;
			if (NavMesh.SamplePosition(target.position, out hit, 50.0f, NavMesh.AllAreas))
			{
				SetDestination(hit.position, 6);
				framesSinceTriggered = 0;
			}

			SetDestination(target.position, 1);

            if (distanceToPlayer < noticeDistance)
            {
                if (handMovement > limit)
                {
                    //The player has moved their hand too much and the fox must flee
                    Debug.Log("CURIOUS: FLEEING because Hands moved too quick");
                    anim.SetBool("isCurious", false);
                    anim.SetBool("isFleeing", true);
                    currentState = STATE.FLEEING;
                }
            }
            else if (distanceToFood < 2.0f)
			{
				//This is for if the fox is close enought to the food
				Debug.Log("CURIOUS: Actually moving to EATING state");
				anim.SetBool("isCurious", false);
				anim.SetBool("isEating", true);

				eatTime = Time.time + 4.0f; //eat animation is 4 seconds long
				currentState = STATE.EATING;
			}
            else if(distanceToFood > noticeDistance)
            {
                Debug.Log("CURIOUS: Food has left the notice distance, moving to IDLE mode");
                anim.SetBool("isCurious", false);
                currentState = STATE.IDLE;
            }
            
            
		}
		else
		{
			Debug.Log("CURIOUS: The foods gone, changing state to IDLE");
			anim.SetBool("isCurious", false);
			currentState = STATE.IDLE;
		}
    }

	private void eatingState()
	{
		Debug.Log("This is Eating State");
        
        if(food == null)
        {
            currentState = STATE.CURIOUS;

        }
        Transform target;
        target = food.transform;

        NavMeshHit hit;
        if((target.position - transform.position).magnitude > 0.6f)
        {
            if (NavMesh.SamplePosition(target.position, out hit, 50.0f, NavMesh.AllAreas))
            {
                 SetDestination(target.position, 1);
                 framesSinceTriggered = 0;
            }
        }
        else
        {
            SetDestination(transform.position, 1);
            //Play any eating animation here
            //The fox is now close enough that ducking down his head should make it look like he's eating
        }

        
        if (distanceToFood < 2.0f)
		{
			if(Time.time > eatTime)
			{
				Debug.Log("EATING: The fox is eating the food, changed to FLEEING");
				Destroy(food);
				anim.SetBool("isEating", false);
				anim.SetBool("isFleeing", true);
				ambianceControl.increaseAmbianceState();
				currentState = STATE.FLEEING; //move to fleeing state once eating is finished.
			}
			else
			{
				if(handMovement > limit)
				{
					Debug.Log("EATING: Player has gotten too close and moved too much, changed to fleeing");
					anim.SetBool("isEating", false);
					anim.SetBool("isFleeing", true);
					currentState = STATE.FLEEING;
				}
			}
		}
		else
		{
			Debug.Log("EATING: Food has moved away from nav, re-entering CURIOUS state");
			anim.SetBool("isEating", false);
			anim.SetBool("isCurious", true);
			currentState = STATE.CURIOUS;
		}

    }

    private void growlState()
    {

        //If not barking && player does not have food
        //Bark

		transform.LookAt(player.transform); //rotate towards the player

		if (!audioSource.isPlaying) 
		{
            //Play the Growl Sound FX
			audioSource.PlayOneShot (growlSound, 1.0f);
		}

		if ((distanceToPlayer < growlDistance) && (distanceToFood > noticeDistance))
        {
			Debug.Log("GROWL: Player has made the fox Flee");
            //Run Away if the Player gets too close without there being food
            anim.SetBool("isGrowling", false);
            anim.SetBool("isFleeing", true);
            currentState = STATE.FLEEING;
        }

        if (food != null)
        {
			if ((handMovement < limit) && (distanceToFood < noticeDistance))
			{
				Debug.Log("GROWL: Player has made the fox Curious");
				//If food exists and is close AND the hands aren't moving too fast, enter curious state
                anim.SetBool("isGrowling", false);
                anim.SetBool("isCurious", true);
                currentState = STATE.CURIOUS;
            }
        }

        if(distanceToPlayer > noticeDistance)
		{
			Debug.Log("GROWL: Player has left the notice area");
			//If the player leaves the notice zone, re-enter the idle state
            anim.SetBool("isGrowling", false);
            currentState = STATE.IDLE;
        }

    }

    private void runningState()
	{
		Debug.Log("Entering the RUNNING state Function");

		SetDestination(AiDestination.position, 7);
		if(flag)
		{
			Debug.Log("RUNNING: State is changing");
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
			SetDestination(AiDestination.position, 7);
			flag = false;
		}
		else
		{
			if(checkDestination(AiDestination.position) == true)
			{
				Debug.Log("RUNNING: Reached the Destination");
                anim.SetBool("isFleeing", false);
				currentState = STATE.IDLE;
				framesSinceTriggered = 0;
				flag = true;
			}
		}


	}

    private void idleState()
    {
        Debug.Log("This is the idle state"); 
		if ( distanceToPlayer < noticeDistance)
		{
			//If the player moves within range, begin growling
			anim.SetBool("isGrowling", true);
			currentState = STATE.GROWL;
		}
		else if (distanceToFood < noticeDistance)
		{
			//Else, if there is food within range, become curious
			anim.SetBool("isCurious", true);
			currentState = STATE.CURIOUS;
		}
        else if (!moving && (framesSinceTriggered > 150))
        {   
			//Else, check for wandering idley around the map
			anim.SetBool("isWalking", true);
            currentState = STATE.WANDER;
            //Move to a random destinatinon
            Transform wanderDestination = AiDestination;

            Vector3 rand = UnityEngine.Random.insideUnitSphere;
            wanderDestination.position = AiDestination.position + (rand*2);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(wanderDestination.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                SetDestination(hit.position, 6);
                framesSinceTriggered = 0;
            }
        }
    }

	private void wanderState()
	{
		//Debug.Log("This is Wander State");
        if (navMeshAgent.remainingDistance < 0.1)
        {
            anim.SetBool("isWalking", false);
            currentState = STATE.IDLE;
        }
		
	}


	private bool checkDestination(Vector3 destination)
	{
		if(Mathf.Abs(this.transform.position.magnitude - destination.magnitude) < 1.0f)
		{
			DistanceToDestination = (Mathf.Abs(this.transform.position.magnitude - destination.magnitude));
			return true;
		}
		else
		{
			DistanceToDestination = (Mathf.Abs(this.transform.position.magnitude - destination.magnitude));
			return false;
		}
	}

    private void SetDestination(Vector3 destination, int speed)
    {
        //Set the new destination in the NavMeshAgent
        if(destination !=null)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.SetDestination(destination);
			target = destination;
        }
    }


    private bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private bool AnimatorIsPlaying(string statename)
    {
        return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(statename);
    }
}
