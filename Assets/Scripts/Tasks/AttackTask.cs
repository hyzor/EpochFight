using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackTask : BaseTask
{
    private Entity entityTarget;
    private Entity entitySrc;
    private Vector3 prevTargetCoords;
    private AttackScript attackScript;

    private bool isAttacking;
    private float lastAttackFinishedTime = 0.0f;

    public enum SubRoutine
    {
        TRAVEL_TO_ATTACK = 0,
        ATTACK = 1
    }

    private SubRoutine curSubroutine;

    public override void OnDestReached()
    {
        if (curSubroutine == SubRoutine.TRAVEL_TO_ATTACK)
        {
            if (TargetIsWithinRange())
            {
                curSubroutine = SubRoutine.ATTACK;
                isBusy = false;
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        curSubroutine = SubRoutine.TRAVEL_TO_ATTACK;
        entityTarget = taskTargetObj.GetComponent<Entity>();
        entitySrc = this.transform.parent.gameObject.GetComponent<Entity>();
        prevTargetCoords = taskTargetObj.transform.position;
        attackScript = this.transform.parent.gameObject.GetComponent<AttackScript>();
        attackScript.targetObj = taskTargetObj;
        isAttacking = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (taskTargetObj == null || !entityTarget.isAlive)
            Destroy(this.gameObject);

        if (!isBusy)
        {
            if (curSubroutine == SubRoutine.TRAVEL_TO_ATTACK)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskTargetObj.transform.position));
                isBusy = true;
            }
            else if (curSubroutine == SubRoutine.ATTACK)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.SetUnitState(Unit.State.ATTACKING));
                StartCoroutine(Attack());
            }
        }

        // Poll this constantly
        if (curSubroutine == SubRoutine.TRAVEL_TO_ATTACK && !TargetIsWithinRange())
        {
            // Target has moved, so order unit to new coordinates
            if (taskTargetObj.transform.position != prevTargetCoords)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitResume());
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskTargetObj.transform.position));
                prevTargetCoords = taskTargetObj.transform.position;
            }
        }

        // If we are attacking, we want to continuously turn ourselves towards the target
        if (isAttacking)
        {
            Vector3 dir = (taskTargetObj.transform.position - this.gameObject.transform.parent.transform.position).normalized;

            // Ignore y dimension
            dir.y = 0.0f;

            Quaternion lookRot = Quaternion.LookRotation(dir);
            this.gameObject.transform.parent.transform.rotation
                = Quaternion.Slerp(this.gameObject.transform.parent.transform.rotation, lookRot, Time.deltaTime * 10.0f);
        }

        // Force unit to stop and start attacking if target is within range
        if (!isAttacking && TargetIsWithinRange())
        {
            ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.SetUnitState(Unit.State.ATTACKING));
            ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitStop());
            StartCoroutine(Attack());
        }
    }

    public bool TargetIsWithinRange()
    {
        float dist = Vector3.Distance(this.transform.parent.gameObject.transform.position, taskTargetObj.transform.position);

        BoxCollider thisBoxCol = this.transform.parent.gameObject.GetComponent<BoxCollider>();
        BoxCollider targetBoxCol = taskTargetObj.GetComponent<BoxCollider>();
        //Debug.Log("Distance to target: " + dist);

        if (thisBoxCol != null && targetBoxCol != null)
        {
            Vector3 closestSrcPoint = thisBoxCol.ClosestPointOnBounds(taskTargetObj.transform.position);
            Vector3 closestTargetPoint = targetBoxCol.ClosestPointOnBounds(this.transform.parent.gameObject.transform.position);
            
            dist = Vector3.Distance(closestSrcPoint, closestTargetPoint);
            //Debug.Log("Distance to target with bounds: " + dist);
        }

        if (dist <= attackScript.range)
        {
            //Debug.Log("Target within range!");
            return true;
        }
        else
            return false;
    }

    private IEnumerator Attack()
    {
        isBusy = true;
        isAttacking = true;

        do
        {
            if (Time.time - lastAttackFinishedTime >= attackScript.cooldown)
            {
                attackScript.BeginAttack();

                yield return new WaitForSeconds(attackScript.duration); // Attack speed

                // Attack is melee, send damage instantly
                if (!attackScript.isRanged)
                {
                    // Is target still within range?
                    if (TargetIsWithinRange())
                    {
                        attackScript.DoAttack();
                        ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(attackScript.damage, entitySrc.gameObject));
                    }
                }
                else
                {
                    attackScript.DoAttack();
                }

                lastAttackFinishedTime = Time.time;
            }
            else
            {
                yield return null;
            }

        } while (taskTargetObj != null && entityTarget.isAlive && TargetIsWithinRange());

        if (taskTargetObj != null)
        {
            if (!entityTarget.isAlive)
                Destroy(this.gameObject);
            else
            {
                if (!TargetIsWithinRange())
                {
                    curSubroutine = SubRoutine.TRAVEL_TO_ATTACK;
                    isBusy = false;
                    isAttacking = false;
                }
            }
        }
        else
        {
            isBusy = false;
            isAttacking = false;
            Destroy(this.gameObject);
        }
    }
}
