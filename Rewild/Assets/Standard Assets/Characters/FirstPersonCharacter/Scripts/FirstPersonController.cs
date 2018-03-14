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
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

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
        private AudioSource m_AudioSource;


		public GameObject handLeft;
		public GameObject handRight;

		public Transform VRCamera;

		public GameObject CameraParent;

        public bool isAnimal = false;
        public bool isTranslating = false;

        public float cameraPanSpeed;
		public Camera scentCamera;
		public GameObject mollieSoundtrack;
        public AudioMixerSnapshot humanMode; // changes the states on the audio mixer depending the state of the player , fox or human.
        public AudioMixerSnapshot animalMode;
        [Range(0, 8)]
        public int transitionTimeInSec; // controls the rate of change between the 2 soundtracks.
        public bool startTransformation;
        public bool foxVision;
    
        private void SetTranslatingToTrue()
        {
            isTranslating = true;
        }
        
        // Use this for initialization
        private void Start()
        {
            foxVision = false;
            startTransformation = false;
            mollieSoundtrack.SetActive(true);

            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

			//handLeft = GameObject.FindGameObjectWithTag("Left Hand");
			//handRight = GameObject.FindGameObjectWithTag("Right Hand");

			scentCamera.enabled = false; //Disable the scent cam initially as player starts as a human

            //Little bit of a cheat hack here, the Cameras starting position does not start at the anchor, so a very very quick transform happens
            isTranslating = true;
            isAnimal = false;
            
        }
        
        // Update is called once per frame
        private void Update()
		{
            
            RotateView();

            // the jump state needs to read here to make sure it is not missed

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                PlayLandingSound();
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
                        
			if (Input.GetButtonDown("Transform 1") && Input.GetButtonDown("Transform 2"))
            {
                if (isTranslating == false)
                {
                    Debug.Log("Switching Forms (V key press or Right Trigger)");

                    if (isAnimal == true)
                    { // switching to human
                        isAnimal = false; // switches to human
						scentCamera.enabled = false;
                        startTransformation = true;
                        Invoke("SetTranslatingToTrue", 5.0f);                        
                    }
                    else if (isAnimal == false)
                    { // switching to animal
                        isAnimal = true; // switches to animal
						scentCamera.enabled = true;
                        startTransformation = true;
                        Invoke("SetTranslatingToTrue", 5.0f);
                    }

                }
               
            }
            
            if ((isTranslating) && (isAnimal == true)) //if the camera is translating TO animal
            {
                
				// Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
				CameraParent.transform.Translate(new Vector3 (0.0f, -0.05f, 0.0f));

				if (CameraParent.transform.localPosition.y < 0.25f)
                { //If Translation has finished and now in Animal mode
                    isTranslating = false;
                    startTransformation = false;
                    foxVision = true;

					handLeft.SetActive(false);
					handRight.SetActive(false);

                    m_CharacterController.height = 0.5f;
					m_CharacterController.slopeLimit = 60;
                    //Enable any visual effects
                    //Switch sounds
                    animalMode.TransitionTo(transitionTimeInSec);
                }

            }
            else if ((isTranslating) && (isAnimal == false)) // if camera is translating TO human
            {
                
				//Debug.Log("Distance: " + Vector3.Distance(transform.position, FirstPersonCameraAnchor.transform.position));
				CameraParent.transform.Translate(new Vector3 (0.0f, 0.05f, 0.0f));

				if (CameraParent.transform.localPosition.y > 1.25f)
				{ //If Translation has finished and now in Mollie mode
                    isTranslating = false;
                    startTransformation = false;
                    foxVision = false;

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
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
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
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
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

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
  //          if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
  //          {
  //              StopAllCoroutines();
 //               StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
 //           }
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