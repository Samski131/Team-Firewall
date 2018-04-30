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

	[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
	[SerializeField] private AudioClip m_Bark;
	[SerializeField] private AudioClip m_Eating;

	[SerializeField] private Transform[] denRoute;

	private float walkSoundTime = 0.0f;

	scr_AmbianceControl ambianceControl;
	NavMeshAgent navMeshAgent;
	//AudioSource audioSource;
	static Animator anim;
	//Public variable to track the foxes movement

	public enum STATE
	{
		FLEEING,
		IDLE,
		CURIOUS,
		EATING,
		GROWL,
		WANDER,
        DEN
	}

	Transform AiDestination;
	public Vector3 target;
	public bool moving = false;
	private int state = 1;
	private bool triggered;
	private int framesSinceTriggered;

	private AudioSource m_AudioSource;
	private AudioSource m_AudioSource_Mollie;

	public float DistanceToDestination;

	private Vector3 lastPosition;
	private SphereCollider colldier;
	public GameObject food;
	public float handMovement = 0.0f;

	public float limit = 20.0f;

	private float noticeDistance = 8.0f;
	private float growlDistance  = 5.0f;
	private float frameCounter = 0.0f;
	public int feedCounter = 0;
	public int routeCounter = 0;

	private GameObject player;
	public STATE currentState;

	private bool flag = true;
	private AudioClip growlSound; //barking for the moment.
	private float eatTime;

	public float distanceToPlayer;
	public float distanceToFood;


	private bool[] VOplayed;
    


	void Start ()
	{
		//Initiliase the variables
		navMeshAgent = this.GetComponent<NavMeshAgent>();
		colldier = this.GetComponent<SphereCollider>();
		ambianceControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_AmbianceControl>();
		player = GameObject.FindGameObjectWithTag("Player");
		m_AudioSource_Mollie = player.GetComponents<AudioSource>()[1];

		anim = GetComponentInChildren<Animator>();
		food = null;
		VOplayed = new bool[13];

		for(int i = 0; i < 13; i++)
		{
			VOplayed[i] = false;
		}

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


		currentState = STATE.FLEEING;

		//Debugging
		if (navMeshAgent == null)
		{
			Debug.Log("No Nav Mesh Agent");
		}

		m_AudioSource = GetComponent<AudioSource>();

	}


	void Update()
	{
		framesSinceTriggered++;
		//anim.SetInteger("State", (int)currentState);
		//update the animator

		if(distanceToPlayer < 20)
		{

			if(!VOplayed[0])
				PlayVOLine(0);

			if(!m_AudioSource_Mollie.isPlaying)
			{
				PlayVOLine(1);

				if(VOplayed[4] && triggered && currentState == STATE.FLEEING)
				{
					PlayVOLine(5);
				}

				triggered = false;
			}
				
		}

		if (ambianceControl.t > 0.5f)
		{
			if(feedCounter == 1)
				PlayVOLine(7);

			if(feedCounter == 2)
				PlayVOLine(9);
		}

		handMovement = player.GetComponent<FirstPersonController>().handMovement;
		distanceToPlayer = (this.transform.position - player.transform.position).magnitude;

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

        // If the fox is moving, calculate the del
        if (moving)
        {
            anim.SetBool("isWalking", true);
            if (Time.time >= walkSoundTime)
            {
                walkSoundTime += 0.2f;
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
        }


        if (feedCounter < 3)
		{
			
			food = GameObject.FindGameObjectWithTag("Food");
			if(food != null)
			{
				distanceToFood = (this.transform.position - food.transform.position).magnitude;
			}
			else
			{
				distanceToFood = 9999990.0f;
			}

			frameCounter++;

			
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

            case STATE.DEN:
                Debug.Log("STATE MACHINE = DEN");
                GoToDen();
                break;
            }
		}
		else
		{

			GoToDen();
		}

	}

	private void curiousState()
	{
		Transform target;

		PlayVOLine(3);

		if(feedCounter == 1)
		{
			PlayVOLine(8);
		}

		if(!m_AudioSource.isPlaying)
			PlayFootStepAudio();

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
					triggered = true;


					currentState = STATE.FLEEING;
				}
			}

			if (distanceToFood < 2.0f)
			{
				//This is for if the fox is close enought to the food
				Debug.Log("CURIOUS: Actually moving to EATING state");
				anim.SetBool("isEating", true);
				anim.SetBool("isCurious", false);

				eatTime = Time.time + 4.0f; //eat animation is 4 seconds long
				currentState = STATE.EATING;
			}

			if(distanceToFood > noticeDistance)
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
			PlayVOLine(6);

			if(feedCounter == 2)
			{
				PlayVOLine(10);
			}


			if(!m_AudioSource.isPlaying)
				m_AudioSource.PlayOneShot(m_Eating);
			
			if(Time.time > eatTime)
			{
				Debug.Log("EATING: The fox is eating the food, changed to FLEEING");

				Destroy(food);
				anim.SetBool("isEating", false);
				anim.SetBool("isFleeing", true);
				ambianceControl.increaseAmbianceState();
				feedCounter++;

                
				currentState = STATE.FLEEING; //move to fleeing state once eating is finished.

			}
			else
			{
				if((handMovement > limit) && (distanceToPlayer < noticeDistance))
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

		PlayVOLine(2);

		if(VOplayed[2] && !m_AudioSource_Mollie.isPlaying)
		{
			PlayVOLine(4);
		}

		//If not barking && player does not have food
		//Bark
		if(!m_AudioSource.isPlaying)
			m_AudioSource.PlayOneShot(m_Bark);
	
		if(frameCounter > 120.0f)
		{
			frameCounter = 0.0f;
		}

		transform.LookAt(player.transform); //rotate towards the player

		//if (!audioSource.isPlaying) 
		//{
			//Play the Growl Sound FX
		//	audioSource.PlayOneShot (growlSound, 1.0f);
		//}

		if ((distanceToPlayer < growlDistance) && (distanceToFood > noticeDistance))
		{
			Debug.Log("GROWL: Player has made the fox Flee");
			//Run Away if the Player gets too close without there being food
			anim.SetBool("isGrowling", false);
			anim.SetBool("isFleeing", true);

			triggered = true;

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


		if(!m_AudioSource.isPlaying)
			PlayFootStepAudio();

		SetDestination(AiDestination.position, 7);
		if(flag && currentState != STATE.DEN)
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
			SetDestination(AiDestination.position, 10);
			flag = false;
		}
		else
		{
            if(currentState == STATE.DEN)
            {
                GoToDen();
            }
            else if(checkDestination(AiDestination.position) == true)
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
				framesSinceTriggered = 0;
				SetDestination(hit.position, 6);
			}
		}
	}

	private void wanderState()
	{
		//Debug.Log("This is Wander State");
		if(!m_AudioSource.isPlaying)
			PlayFootStepAudio();

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

	private void PlayFootStepAudio()
	{
		// pick & play a random footstep sound from the array,
		// excluding sound at index 0
		int n = Random.Range(1, m_FootstepSounds.Length);
		m_AudioSource.clip = m_FootstepSounds[n];
		m_AudioSource.volume = (25/distanceToPlayer);
		m_AudioSource.PlayOneShot(m_AudioSource.clip);
		// move picked sound to index 0 so it's not picked next time
		m_FootstepSounds[n] = m_FootstepSounds[0];
		m_FootstepSounds[0] = m_AudioSource.clip;
	}

	public void PlayVOLine(int VOElementeNumber)
	{
		if(!VOplayed[VOElementeNumber])
		{
			if (m_AudioSource_Mollie.isPlaying)
			{
					m_AudioSource_Mollie.Stop();
			}

			m_AudioSource_Mollie.PlayOneShot(player.GetComponent<FirstPersonController>().m_CrucialVoiceLines[VOElementeNumber]);

			VOplayed[VOElementeNumber] = true;
		}

		player.GetComponent<FirstPersonController>().VOCounter = 99;
	}

	private void GoToDen()
	{

        anim.SetBool("isFleeing", true);
        anim.SetBool("isDen", true);
        anim.SetBool("isEating", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isGrowling", false);
        anim.SetBool("isCurious", false);


        if (routeCounter < 4)
		{
			
			if(!m_AudioSource.isPlaying)
			{
				PlayFootStepAudio();
			}
            
			NavMeshHit hit;
            
			NavMesh.SamplePosition(denRoute[routeCounter].position, out hit, 2.0f, NavMesh.AllAreas);
			SetDestination(hit.position, 10);

			if(checkDestination(hit.position))
			{
				routeCounter++;
				Debug.Log("At Position");
				NavMesh.SamplePosition(denRoute[routeCounter].position, out hit, 2.0f, NavMesh.AllAreas);
				SetDestination(hit.position, 10);
			}
		}
		else
		{
			if(distanceToPlayer < 10)
			{
				PlayVOLine(11);

				if(VOplayed[11] && !m_AudioSource_Mollie.isPlaying)
				{
					PlayVOLine(12);
				}

			}
			//Make Nav Disappear here (Hes reached the burrow)
			Debug.Log(gameObject.transform.GetChild(1).name);
			gameObject.transform.GetChild(1).gameObject.SetActive(false);
		}
	}
}