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
    public bool moving = false;
	public int state = 1;
    public bool triggered;
	public int framesSinceTriggered;

	public Vector3 lastPosition;
	private SphereCollider colldier;
	public GameObject food;
    private float handMovement = 0.0f;

    public float limit = 20.0f;

    public float noticeDistance = 8.0f;
    public float growlDistance  = 5.0f;


    private GameObject player;
	public STATE currentState;

	public bool flag = true;
	public AudioClip growlSound; //barking for the moment.
    private float eatTime;


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

        lastPosition = this.transform.position;

        AiDestination = destination;
        SetDestination(AiDestination.position, 5);
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
        //anim.SetInteger("State", (int)currentState); //update the animator
       
       

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


        handMovement = player.GetComponent<FirstPersonController>().handMovement;
        float distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
        food = GameObject.FindGameObjectWithTag("Food");
        if (currentState != STATE.FLEEING && currentState != STATE.EATING) // can't interupt if fleeing or eating
        {
            if (distanceToPlayer >= noticeDistance)
            {
                Debug.Log("IDLE");
                anim.SetBool("isGrowling", false);
                anim.SetBool("isFleeing", false);
                anim.SetBool("isCurious", false);
                currentState = STATE.IDLE;
            }

            if (distanceToPlayer < noticeDistance && distanceToPlayer >= growlDistance && food == null) //only triggers if in the notice zone when there is no food in the level
            {
                Debug.Log("Growling");
                anim.SetBool("isGrowling", true);
                currentState = STATE.GROWL;
            }
            else if (food != null)
            {
                Debug.Log("There is food!");
                float distanceToFood = (this.transform.position - food.transform.position).magnitude;
                if (distanceToFood < noticeDistance)
                {
                    Debug.Log("Curious");
                    anim.SetBool("isGrowling", false);
                    anim.SetBool("isCurious", true);
                    anim.SetBool("isWalking", false);
                    currentState = STATE.CURIOUS;
                }
            }



            if ((distanceToPlayer < growlDistance) && (currentState != STATE.CURIOUS)) // if the player is too close to the fox, the fox flees, unless the fox is curious
            {
                Debug.Log("Fleeing");
                anim.SetBool("isGrowling", false);
                anim.SetBool("isCurious", false);
                anim.SetBool("isEating", false);
                anim.SetBool("isFleeing", true);
                currentState = STATE.FLEEING;
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
				idleState();
				break;

            case STATE.GROWL:
                growlState();
                break;
            case STATE.WANDER:
                wanderState();
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
            SetDestination(target.position, 1);
			Debug.Log ((this.transform.position - food.transform.position).magnitude);
            if ((this.transform.position - food.transform.position).magnitude < 1.0f)
            {
				Debug.Log("Actually moving to eating state");
                //This is for if the fox is close enought to the food
                anim.SetBool("isCurious", false);
                anim.SetBool("isEating", true);
                currentState = STATE.EATING;
            }
        }
        else
        {
            //The player has moved their hand too much and the fox must flee
            anim.SetBool("isCurious", false);
            anim.SetBool("isFleeing", true);
            currentState = STATE.FLEEING;
        }
        //Get the location of the food
        //Check that the player isnt around
        //Move the fox slowly towards the food
    }

	private void eatingState()
	{
        Debug.Log("This is Eating State");
        ambianceControl.increaseAmbianceState();

        eatTime = Time.time + 4.0f; //eat animation is 4 seconds long

        Destroy(food);
        anim.SetBool("isFleeing", true);
        anim.SetBool("isEating", false);
        currentState = STATE.FLEEING; //move to fleeing state once eating is finished.
            

    }

    private void growlState()
    {

        //If not barking && player does not have food
        //Bark
		transform.LookAt(player.transform); //rotate towards the player

		if (!audioSource.isPlaying) 
		{
			audioSource.PlayOneShot (growlSound, 1.0f);
		}

        float distanceToPlayer = (this.transform.position - player.transform.position).magnitude;
        if (distanceToPlayer < growlDistance)
        {
            //Run Away
            anim.SetBool("isGrowling", false);
            anim.SetBool("isFleeing", true);
            currentState = STATE.FLEEING;
        }
        if (food != null)
        {
            if (handMovement < limit && ((food.transform.position - this.transform.position).magnitude < noticeDistance))
            {
                anim.SetBool("isGrowling", false);
                anim.SetBool("isCurious", true);
                currentState = STATE.CURIOUS;
            }
        }

        if(distanceToPlayer > noticeDistance)
        {
            anim.SetBool("isGrowling", false);
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
			SetDestination(AiDestination.position, 7);
			flag = false;
		}
		else
		{
			if(checkDestination(AiDestination.position) == true)
			{
                anim.SetBool("isFleeing", false);
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

    private void idleState()
    {
        Debug.Log("This is the idle state"); 
        if (!moving && (framesSinceTriggered > 150))
        {   
            currentState = STATE.WANDER;
            anim.SetBool("isWalking", true);
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
