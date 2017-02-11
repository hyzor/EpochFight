using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IClickable, IUnitMessageHandler
{
    public enum State
    {
        IDLE = 0,
        TRAVELING = 1,
        WORKING = 2,
        COMBAT = 3
    }

    public int health = 1;
    public float speed = 1.0f;

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private TextMesh textMesh;
    private State curState;

	// Use this for initialization
	void Start () {
        taskMgr = GetComponent<UnitTaskManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        curState = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
        if (taskMgr.TaskIsActive())
        {
            if (HasReachedDestination())
            {
                taskMgr.OnDestReached();
                Debug.Log(this.gameObject.name + " reached its destination!");
            }
        }
    }

    public void OnLeftClick()
    {
        Debug.Log("Unit left clicked!");
    }

    public void OnRightClick()
    {
        Debug.Log("Unit right clicked!");
    }

    public void OrderUnitToCoords(Vector3 coords)
    {
        navMeshAgent.SetDestination(coords);
    }

    public bool HasReachedDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0.0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
