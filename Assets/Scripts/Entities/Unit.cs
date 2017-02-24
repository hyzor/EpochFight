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

    private UnitTaskManager taskMgr;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private Entity entity;
    public State curState;
    public int statusTextIndex = 1;
    private AttackScript attackScript;
    private DetectionCollider detCol;
    private bool isPlayerUnit;

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
        attackScript = this.gameObject.GetComponent<AttackScript>();
        detCol = gameObject.GetComponentInChildren<DetectionCollider>();

        if (gameObject.GetComponent<Enemy>() == null)
            isPlayerUnit = true;
        else
            isPlayerUnit = false;
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
            if (taskMgr != null)
                taskMgr.AbortCurrentTask();

            if (navMeshAgent != null)
                navMeshAgent.Stop();

            OnDie();
        }

        if (entity.isAlive && !HasTaskAssigned())
        {
            // Try to find an attackable target
           if (FindTargetAndAttack())
                Debug.Log(this.name + " attacking!");
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
            anim.speed = 1.0f;

            if (attackScript != null)
            {
                if (attackScript.attackType == AttackScript.AttackType.MELEE_ONEHANDED)
                {
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 0);
                    anim.SetInteger("Animation_int", 2);
                }
                else if (attackScript.attackType == AttackScript.AttackType.RANGED_BOW)
                {
                    anim.SetInteger("MeleeType_int", 1);
                    anim.SetInteger("WeaponType_int", 11);
                    anim.SetInteger("Animation_int", 0);
                }
            }
            else
            {
                anim.SetInteger("MeleeType_int", 1);
                anim.SetInteger("WeaponType_int", 0);
                anim.SetInteger("Animation_int", 2);
            }
        }
        else if (curState == State.TRAVELING)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Traveling)");
            anim.speed = 1.0f;
            anim.SetFloat("Speed_f", 1.0f);
            anim.SetInteger("MeleeType_int", 12);
            anim.SetInteger("WeaponType_int", 12);
            anim.SetInteger("Animation_int", 0);
            anim.SetBool("Shoot_b", false);
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
        }
        else if (curState == State.DEAD)
        {
            entity.InsertStatusTextElement(statusTextIndex, " (Dead)");
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 0);
            anim.SetBool("Shoot_b", false);
        }
        else
        {
            anim.SetFloat("Speed_f", 0.0f);
            anim.SetInteger("MeleeType_int", 1);
            anim.SetInteger("WeaponType_int", 0);
            anim.SetInteger("Animation_int", 0);
        }
    }

    public BaseTask.TaskType GetCurTaskType()
    {
        return taskMgr.GetCurrentTask().taskType;
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

    public bool HasTaskAssigned()
    {
        if (taskMgr.GetCurrentTask() != null)
            return true;

        return false;
    }

    public bool HasActiveTask()
    {
        return taskMgr.TaskIsActive();
    }

	public void OnLeftClick(Vector3 point)
    {
        //Debug.Log("Unit left clicked!");
    }

	public void OnRightClick(Vector3 point)
    {
        //Debug.Log("Unit right clicked!");
    }

    public void OrderUnitToCoords(Vector3 coords)
    {
        if (entity.isAlive)
        {
            navMeshAgent.SetDestination(coords);
            curState = State.TRAVELING;
        }
    }

    public void OrderAttackOn(GameObject target)
    {
        if (target == null || !target.GetComponent<Entity>().isAlive)
            return;

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.RequestSetTask(BaseTask.TaskType.ATTACK));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationCoords(target.transform.position));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationObj(target));
    }

    public bool FindTargetAndAttack()
    {
        GameObject targetObj = null;

        DetectionCollider.ObjTypes firstPrio;
        DetectionCollider.ObjTypes secondPrio;
        DetectionCollider.ObjTypes thirdPrio;

        if (isPlayerUnit)
        {
            firstPrio = DetectionCollider.ObjTypes.UNIT_ENEMY;
            secondPrio = DetectionCollider.ObjTypes.BASE_ENEMY;
            thirdPrio = DetectionCollider.ObjTypes.BUILDING_ENEMY;
        }
        else
        {
            firstPrio = DetectionCollider.ObjTypes.UNIT;
            secondPrio = DetectionCollider.ObjTypes.BASE;
            thirdPrio = DetectionCollider.ObjTypes.BUILDING;

        }

        // Is there an enemy unit in sight? (First prio)
        if (detCol.objectsInSight.ContainsKey(firstPrio))
        {
            targetObj = detCol.objectsInSight[firstPrio];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(firstPrio);
                return false;
            }
        }

        // Is there an enemy base in sight? (Second prio)
        else if (detCol.objectsInSight.ContainsKey(secondPrio))
        {
            targetObj = detCol.objectsInSight[secondPrio];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(secondPrio);
                return false;
            }
        }

        // Is there an enemy building in sight? (Third prio)
        else if (detCol.objectsInSight.ContainsKey(thirdPrio))
        {
            targetObj = detCol.objectsInSight[thirdPrio];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(thirdPrio);
                return false;
            }
        }

        if (targetObj != null)
        {
            OrderAttackOn(targetObj);
            return true;
        }

        return false;
    }

    public void OrderUnitStop()
    {
        navMeshAgent.Stop();
    }

    public void OrderUnitResume()
    {
        navMeshAgent.Resume();
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
