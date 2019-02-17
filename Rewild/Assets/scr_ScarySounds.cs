using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ScarySounds : MonoBehaviour
{
	[SerializeField] private AudioClip[] ambientSounds;
	[SerializeField] private GameObject[] emitters;
	private AudioSource[] m_emitters;

	public float timeSinceLastSFX = 0.0f;

	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < emitters.Length; i++)
		{
			emitters[i].GetComponent<AudioSource>().volume = 0.1f;
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeSinceLastSFX += 0.1f;

		if(timeSinceLastSFX > 60.0f)
		{
			PlayAmbientSound();
		}
		
	}

	private void PlayAmbientSound()
	{

		timeSinceLastSFX = 0.0f;

		// pick & play a random footstep sound from the array,
		// excluding sound at index 0
		int rand_Emitter = Random.Range(1, emitters.Length);
		int rand_Sound = Random.Range(1, ambientSounds.Length);

		emitters[rand_Emitter].GetComponent<AudioSource>().clip = ambientSounds[rand_Sound];
		emitters[rand_Emitter].GetComponent<AudioSource>().PlayOneShot(ambientSounds[rand_Sound]);


		//m_AudioSource.clip = m_FootstepSounds[n];
		//m_AudioSource.PlayOneShot(m_AudioSource.clip);

		// move picked sound to index 0 so it's not picked next time
		//m_FootstepSounds[n] = m_FootstepSounds[0];
		//m_FootstepSounds[0] = m_AudioSource.clip;
	}

	public void PlaySpecificSound(int soundNumber)
	{
		int rand_Emitter = Random.Range(1, emitters.Length);
		emitters[rand_Emitter].GetComponent<AudioSource>().clip = ambientSounds[soundNumber];
		emitters[rand_Emitter].GetComponent<AudioSource>().PlayOneShot(ambientSounds[soundNumber]);
	}
}