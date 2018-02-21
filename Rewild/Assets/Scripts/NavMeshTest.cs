using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{

    [SerializeField]
    Transform destination;
    [SerializeField]
    Transform destination2;
    [SerializeField]
    Transform destination3;

    NavMeshAgent navMeshAgent;

    private float velocity;

    private bool moving = false;
    private int state = 1;
    private Vector3 lastPosition;

	void Start ()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
 
        lastPosition = transform.position;

        if (navMeshAgent == null)
        {
            Debug.Log("No Nav Mesh Agent");
        }
		
	}

    void Update()
    {

        if (transform.position != lastPosition)
        {
            moving = true;
            lastPosition = transform.position;
        }
        else
        {
            moving = false;
            lastPosition = transform.position;
        }
        
       if(!moving)
        {
            if (state == 1)
            {
                state = 2;
            SetDestination1();
            }
                
            else if (state == 2)
            {
                state = 3;
                SetDestination2();
            }
            else if (state == 3)
            {
                state = 1;
                SetDestination3();
            }

            moving = true;
            Debug.Log(state);
        }
    }
	
    private void SetDestination1()
    {

        if(destination !=null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
    private void SetDestination2()
    {

        if (destination2 != null)
        {
            Vector3 targetVector = destination2.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
    private void SetDestination3()
    {

        if (destination3 != null)
        {
            Vector3 targetVector = destination3.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

}
