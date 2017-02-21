using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackTask : BaseTask
{
    private Entity entityTarget;
    private Entity entitySrc;
    private Vector3 prevTargetCoords;

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

        if (curSubroutine == SubRoutine.TRAVEL_TO_ATTACK && !TargetIsWithinRange())
        {
            // Target has moved, so order unit to new coordinates
            if (taskTargetObj.transform.position != prevTargetCoords)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskTargetObj.transform.position));
                prevTargetCoords = taskTargetObj.transform.position;
            }
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

        if (dist <= entitySrc.attackRange)
            return true;
        else
            return false;
    }

    private IEnumerator Attack()
    {
        isBusy = true;

        do
        {
            yield return new WaitForSeconds(entitySrc.attackSpeed); // Attack speed
            ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(entitySrc.attackDamage, entitySrc.gameObject));
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
                }
            }
        }
        else
        {
            isBusy = false;
            Destroy(this.gameObject);
        }
    }
}
