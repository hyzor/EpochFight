using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackTask : BaseTask {

    private Entity entityTarget;

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
            curSubroutine = SubRoutine.ATTACK;
            isBusy = false;
        }
    }

    // Use this for initialization
    void Start ()
    {
        curSubroutine = SubRoutine.TRAVEL_TO_ATTACK;
        entityTarget = taskTargetObj.GetComponent<Entity>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!entityTarget.isAlive)
            Destroy(this.gameObject);

		if (!isBusy)
        {
            if (curSubroutine == SubRoutine.TRAVEL_TO_ATTACK)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskCoords));
                isBusy = true;
            }
            else if (curSubroutine == SubRoutine.ATTACK)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.SetUnitState(Unit.State.ATTACKING));
                StartCoroutine(Attack());
            }
        }
	}

    private IEnumerator Attack()
    {
        isBusy = true;

        do
        {
            yield return new WaitForSeconds(1); // Attack speed
            ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(1, this.transform.parent.gameObject));
        } while (taskTargetObj != null && entityTarget.isAlive);

        isBusy = false;
        Destroy(this.gameObject);
    }
}
