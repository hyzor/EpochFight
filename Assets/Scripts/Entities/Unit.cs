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
        ATTACKING = 3,
        DEAD = 4
    }

    public float speed = 1.0f;

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private Entity entity;
    public State curState;
    public int statusTextIndex = 1;

	// Use this for initialization
	void Start () {
        taskMgr = this.gameObject.GetComponent<UnitTaskManager>();
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        anim = this.gameObject.GetComponent<Animator>();
        entity = this.gameObject.GetComponent<Entity>();
        curState = State.IDLE;
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetInteger("MeleeType_int", 1);
        anim.SetInteger("WeaponType_int", 0);
        anim.SetInteger("Animation_int", 2);
        anim.SetBool("Static_b", true);
    }

    public void OnDie()
    {
        entity.isAlive = false;

        // Begin death animation
        anim.SetBool("Death_b", true);
        anim.SetInteger("DeathType_int", 1); // Play "Death_01" animation
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetInteger("MeleeType_int", 1);
        anim.SetInteger("WeaponType_int", 0);
        anim.SetInteger("Animation_int", 2);

        // Get "Death" layer state info
        AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(4);

        // If the death animation has finished playing...
        if (animState.IsName("Dead_01"))
        {
            // ...we flag this entity for removal
            entity.flaggedForRemoval = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (entity.deathTrigger && !entity.flaggedForRemoval)
        {
            taskMgr.AbortCurrentTask();
            navMeshAgent.Stop();
            OnDie();
        }
            

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
        else if (entity.deathTrigger)
        {
            curState = State.DEAD;
        }
        else
        {
            curState = State.IDLE;
        }

        if (curState == State.IDLE)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Idle)");
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 2);
        }
        else if (curState == State.TRAVELING)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Traveling)");
            anim.SetFloat("Speed_f", 1.0f);
            anim.SetInteger("MeleeType_int", 12);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.WORKING)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Working)");
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.ATTACKING)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Attacking)");
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.DEAD)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Dead)");
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 0);
        }
        else
        {
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 0);
        }
    }

    public void OnReceiveDamage(GameObject src)
    {
        // Do not attack back if we are already attacking something or traveling somewhere
        if (curState != State.ATTACKING && curState != State.TRAVELING)
        {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.ATTACK));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.SetTaskDestinationCoords(src.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.SetTaskDestinationObj(src));
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
        if (entity.isAlive)
        {
            navMeshAgent.SetDestination(coords);
            curState = State.TRAVELING;
        }
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
