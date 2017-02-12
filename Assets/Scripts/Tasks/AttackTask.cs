using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackTask : BaseTask {

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
	}
	
	// Update is called once per frame
	void Update ()
    {
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
            ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(1));
        } while (taskTargetObj != null);

        isBusy = false;
        Destroy(this.gameObject);
    }
}
