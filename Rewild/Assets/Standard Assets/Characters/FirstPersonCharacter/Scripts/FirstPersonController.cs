using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] public float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private float m_StepInterval;


        [SerializeField] private AudioClip[] m_FootstepSounds;
		[SerializeField] private AudioClip[] m_IntroLines;
		[SerializeField] public AudioClip[] m_CrucialVoiceLines;  
		[SerializeField] public AudioClip[] m_FillerVoiceLines;  

		[SerializeField] private AudioClip m_TwigSound;   
		[SerializeField] private AudioClip m_LandSound;     

		private AudioSource m_AudioSource_sfx;
		private AudioSource m_AudioSource_vo;
		private float VOTimeCurrent = 0.0f;
		private float VOTimeStart = 0.0f;
		public float rumbleStrength = 0.0f;

        private Camera m_Camera;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;

		public int VOCounter = 6;
        public GameObject CameraParent;
        public Transform VRCamera;
        public GameObject handLeft;
        public GameObject handRight;

		public GameObject nav;
		public float distanceToPlayerFPS; 

        public float handMovement = 0.0f;
        private Vector3 lastRightHandPosition;
        private Vector3 lastLeftHandPosition;

        public bool isAnimal = false;
        public bool isTranslating = false;

        public float cameraPanSpeed;
		public Camera scentCamera;
        public AudioMixerSnapshot humanMode; // changes the states on the audio mixer depending the state of the player , fox or human.
        public AudioMixerSnapshot animalMode;
        [Range(0, 8)]
        public int transitionTimeInSec; // controls the rate of change between the 2 soundtracks.
        public bool startTransformation;
        public bool foxVision;
        public bool triggerTransformation = false;
		private bool foxPowerGained = false;
    
        public void SetTranslatingToTrue()
        {
            isTranslating = true;
        }
        
        // Use this for initialization
        private void Start()
        {
            foxVision = false;
            startTransformation = false;
           
            m_CharacterController = GetComponent<CharacterController>();

            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0.0f;
            m_NextStep = m_StepCycle/2f;
            m_AudioSource_sfx = GetComponents<AudioSource>()[0];
			m_AudioSource_vo = GetComponents<AudioSource>()[1];

			m_MouseLook.Init(transform , m_Camera.transform);

            handLeft.SetActive(true);
            handRight.SetActive(true);


            lastRightHandPosition = handRight.transform.position;
            lastLeftHandPosition = handLeft.transform.position;


            scentCamera.enabled = false; //Disable the scent cam initially as player starts as a human

            //Little bit of a cheat hack here, the Cameras starting position does not start at the anchor, so a very very quick transform happens
            isTranslating = true;
            isAnimal = false;
            
        }
        
        // Update is called once per frame
        private void Update()
		{
            RotateView();
			distanceToPlayerFPS = (nav.transform.position - transform.position).magnitude;
		

			if(VOCounter < 7)
			{
				IntroDialogue();
			}


			//THis code allows the camera to go much closer to the ground, however it does break the hill climbing stuff, Maybe store the "true" height as another variable? Or move the CameraParent rather than the Player
			//Vector3 temp = transform.position;
			//temp.y = 0.0f;
			//Quaternion tempRotation = transform.rotation;
			//transform.SetPositionAndRotation(temp, tempRotation);

			m_CharacterController.height = GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition.y;

			//CameraParent.transform.position.x += GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition.x;
			//CameraParent.transform.position.z += GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition.z;

            float currentFrameMovement = ((handRight.transform.position - lastRightHandPosition).magnitude + (handLeft.transform.position - lastLeftHandPosition).magnitude)*100.0f;

            handMovement = ((handMovement + currentFrameMovement) - (handMovement/5));
            lastRightHandPosition = handRight.transform.position;
            lastLeftHandPosition = handLeft.transform.position;

            // the jump state needs to read here to make sure it is not missed

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayLandingSound();
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            if ((Input.GetButtonDown("Transform 1") && Input.GetButtonDown("Transform 2")))// && foxPowerGained) || triggerTransformation)
            {
				foxPowerGained = true;
				Debug.Log("SAM CAN PRESS BUTTONS");
                if (isTranslating == false)
                {
                    Debug.Log("Switching Forms (V key press or Right Trigger)");

                    if (isAnimal == true)
                    { // switching to human
                        isAnimal = false; // switches to human
						scentCamera.enabled = false;
                        startTransformation = true;
                        triggerTransformation = false;
                        Invoke("SetTranslatingToTrue", 3.0f);
                    }
                    else if (isAnimal == false)
                    { // switching to animal
                        isAnimal = true; // switches to animal
						scentCamera.enabled = true;
                        startTransformation = true;
                        triggerTransformation = false;
                        Invoke("SetTranslatingToTrue", 3.0f);
                    }

                }
               
            }

            if ((isTranslating) && (isAnimal == true)) //if transforming TO animal
            {

                CameraParent.transform.Translate(new Vector3(0.0f, -0.05f, 0.0f));

                Debug.Log("Toggle Transformation");
                if (CameraParent.transform.localPosition.y < 0.25f)
                { //If Translation has finished and now in Animal mode
                    isTranslating = false;
                    startTransformation = false;
                    foxVision = true;
                    triggerTransformation = false;

                    handLeft.SetActive(false);
                    handRight.SetActive(false);

                    m_CharacterController.height = 0.5f;
                    m_CharacterController.slopeLimit = 60;
                    //Enable any visual effects
                    //Switch sounds
                    animalMode.TransitionTo(transitionTimeInSec);
                }

            }
            else if ((isTranslating) && (isAnimal == false)) // if transforming TO human
            {

                CameraParent.transform.Translate(new Vector3(0.0f, 0.05f, 0.0f));

                if (CameraParent.transform.localPosition.y > 1.25f)
                { //If Translation has finished and now in Mollie mode
                    isTranslating = false;
                    startTransformation = false;
                    foxVision = false;
                    triggerTransformation = false;

                    handLeft.SetActive(true);
                    handRight.SetActive(true);

                    m_CharacterController.height = 1.8f;
                    m_CharacterController.slopeLimit = 60;
                    //Disable any visual effects
                    //Switch sound
                    humanMode.TransitionTo(transitionTimeInSec);
                }
            }



            m_CharacterController.center = m_Camera.transform.localPosition;
			//m_CharacterController.transform.rotation = m_Camera.transform.rotation;
                        
        }
        
        private void PlayLandingSound()
        {
            m_AudioSource_sfx.clip = m_LandSound;
			m_AudioSource_sfx.Play();
            m_NextStep = m_StepCycle + .5f;
        }
        
        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

			//VRCamera.TransformDirection (Vector3.forward);
            // always move along the camera forward as it is the direction that it being aimed at

			//This is the line of code that decides that the movement vector. Remove the (+m_Camera.transform.right*m_Input.x;) to remove sidewards movement
			Vector3 desiredMove = m_Camera.transform.forward*m_Input.y + m_Camera.transform.right*m_Input.x;
            
            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


			//Falling
            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }



            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);

            m_MouseLook.UpdateCursorLock();
        }
        
        
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }
        
        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
			m_AudioSource_sfx.clip = m_FootstepSounds[n];
			m_AudioSource_sfx.PlayOneShot(m_AudioSource_sfx.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
			m_FootstepSounds[0] = m_AudioSource_sfx.clip;
        }

		private void IntroDialogue()
		{

			if(!m_AudioSource_vo.isPlaying)
			{
				VOTimeStart = Time.fixedTime;
				Debug.Log("VO");
				m_AudioSource_vo.PlayOneShot(m_IntroLines[VOCounter]);
				VOCounter++;
			}

			if(VOCounter == 7)
			{
				if((Time.fixedTime - VOTimeStart) > 10.5f)
				{
						
						m_AudioSource_sfx.PlayOneShot(m_TwigSound);
						VOTimeStart = 99.000f;
				}

			}
		}

	
        
        private void GetInput(out float speed)
        {
            // Read input 
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
			m_IsWalking = !Input.GetButton("Sprint");
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
            
        }
        
        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }
              
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }    
}

// modified by: Panagiotis Katsiadramis 6/02/18
// modified by: Panagiotis Katsiadramis 13/02/18
// modified by: Panagiotis Katsiadramis 19/02/18