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
        FIGHTING = 3
    }

    public int health = 1;
    public float speed = 1.0f;

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private TextMesh textMesh;
    private Animator anim;
    private State curState;

	// Use this for initialization
	void Start () {
        taskMgr = GetComponent<UnitTaskManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        anim = GetComponent<Animator>();
        curState = State.IDLE;
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetInteger("MeleeType_int", 1);
        anim.SetInteger("WeaponType_int", 0);
        anim.SetInteger("Animation_int", 2);
        anim.SetBool("Static_b", true);
    }

    // Update is called once per frame
    void Update () {
        if (taskMgr.TaskIsActive())
        {
            if (curState == State.TRAVELING)
            {
                if (HasReachedDestination())
                {
                    curState = State.IDLE;
                    taskMgr.OnDestReached();
                    Debug.Log(this.gameObject.name + " reached its destination!");
                }
            }
        }
        else
        {
            curState = State.IDLE;
        }

        if (curState == State.IDLE)
        {
            textMesh.text = "Idle";
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 2);
        }
        else if (curState == State.TRAVELING)
        {
            textMesh.text = "Traveling";
            anim.SetFloat("Speed_f", 1.0f);
            anim.SetInteger("MeleeType_int", 12);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.WORKING)
        {
            textMesh.text = "Working";
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.FIGHTING)
        {
            // Fighting animation
        }
        else
        {
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 1);
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
        curState = State.TRAVELING;
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

    public void SetUnitState(State state)
    {
        curState = state;
    }
}
