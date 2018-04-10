//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_TestThrow : MonoBehaviour
{
	public GameObject prefab;
	public Rigidbody attachPoint;

	SteamVR_TrackedObject trackedObj;
	FixedJoint joint;

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	void FixedUpdate()
	{
		var device = SteamVR_Controller.Input((int)trackedObj.index);
		if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("Button touch");
			if (GameObject.FindGameObjectWithTag("Food"))
			{
				Destroy(GameObject.FindGameObjectWithTag("Food"));
				Debug.Log("Destroyed Food");
			}

			Debug.Log("Create Food");
			var go = GameObject.Instantiate(prefab);
			go.transform.position = attachPoint.transform.position;
			go.GetComponent<BoxCollider>().enabled = false;
			joint = go.AddComponent<FixedJoint>();
			joint.connectedBody = attachPoint;
		}
		else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			var go = joint.gameObject;
			go.GetComponent<BoxCollider>().enabled = true;
			var rigidbody = go.GetComponent<Rigidbody>();
			Object.DestroyImmediate(joint);
			joint = null;
			//Object.Destroy(go, 15.0f); //destroy thrown object after 15s

			// We should probably apply the offset between trackedObj.transform.position
			// and device.transform.pos to insert into the physics sim at the correct
			// location, however, we would then want to predict ahead the visual representation
			// by the same amount we are predicting our render poses.

			var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
			if (origin != null)
			{
				rigidbody.velocity = origin.TransformVector(device.velocity * 1.4f);
				rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity* 1.4f);
			}
			else
			{
				rigidbody.velocity = device.velocity* 1.4f;
				rigidbody.angularVelocity = device.angularVelocity* 1.4f;
			}

			rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude* 1.4f;
		}
	}
}
