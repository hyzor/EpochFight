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
        ATTACKING = 3
    }

    public float speed = 1.0f;

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private TextMesh textMesh;
    private Animator anim;
    private Entity entity;
    public State curState;

	// Use this for initialization
	void Start () {
        taskMgr = GetComponent<UnitTaskManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        textMesh = (TextMesh)GetComponentInChildren(typeof(TextMesh));
        anim = GetComponent<Animator>();
        entity = GetComponent<Entity>();
        curState = State.IDLE;
        anim.SetFloat("Speed_f", 0.0f);
        anim.SetInteger("MeleeType_int", 1);
        anim.SetInteger("WeaponType_int", 0);
        anim.SetInteger("Animation_int", 2);
        anim.SetBool("Static_b", true);
        navMeshAgent.stoppingDistance = 1.0f; // TODO: Adjust this value w.r.t. the units' dimensions
    }

    public void OnDie()
    {
        // Play death animation
        anim.SetBool("Death_b", true);
        anim.SetInteger("DeathType_int", 1);

        // Wait for death animation to finish
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death_01"))
            return;

        // Set death flag to true for removal
        entity.isAlive = false;
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update () {
        if (entity.deathTrigger)
        {
            OnDie();
            return;
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
        else
        {
            curState = State.IDLE;
        }

        textMesh.text = entity.curHealth + "/" + entity.maxHealth;

        if (curState == State.IDLE)
        {
            textMesh.text += " (Idle)";
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 2);
        }
        else if (curState == State.TRAVELING)
        {
            textMesh.text += " (Traveling)";
            anim.SetFloat("Speed_f", 1.0f);
            anim.SetInteger("MeleeType_int", 12);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.WORKING)
        {
            textMesh.text += " (Working)";
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else if (curState == State.ATTACKING)
        {
            textMesh.text += " (Attacking)";
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
        }
        else
        {
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 1);
        }
    }

    public void OnReceiveDamage(GameObject src)
    {
        // Is the unit already attacking something?
        if (curState != State.ATTACKING)
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
