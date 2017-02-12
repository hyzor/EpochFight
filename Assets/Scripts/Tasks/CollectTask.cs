using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectTask : BaseTask
{
    public int numResources;
    public int maxResources;
    public int collectingTime;
    private GameObject resourceCanvasElement;

    public enum SubRoutine
    {
        TRAVEL_TO_COLLECT = 0,
        TRAVEL_TO_DEPOSIT = 1,
        COLLECT = 2
    }

    private SubRoutine curSubroutine;

    public void Start()
    {
        maxResources = 5;
        collectingTime = 1;
        numResources = 0;
        curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
        Transform canvasResourceTextTrans = GameObject.Find("Canvas").transform.FindChild("ResourceText");
        resourceCanvasElement = canvasResourceTextTrans.gameObject;
    }

    public override void OnDestReached()
    {
        if (curSubroutine == SubRoutine.TRAVEL_TO_COLLECT)
        {
            curSubroutine = SubRoutine.COLLECT;
            isBusy = false;
        }
        else if (curSubroutine == SubRoutine.TRAVEL_TO_DEPOSIT)
        {
            ExecuteEvents.Execute<ICanvasMessageHandler>(resourceCanvasElement, null, (x, y) => x.IncrementComponentValue(numResources));
            numResources = 0;
            isBusy = false;

            if (taskTargetObj != null)
                curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
            else
                Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        // Take appropriate action according to the sub-routine
        if (!isBusy)
        {
            if (curSubroutine == SubRoutine.TRAVEL_TO_COLLECT)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(taskCoords));
                isBusy = true;
            }

            else if (curSubroutine == SubRoutine.COLLECT)
            {
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.SetUnitState(Unit.State.WORKING));
                StartCoroutine(CollectResource());
            }

            else if (curSubroutine == SubRoutine.TRAVEL_TO_DEPOSIT)
            {
                // Travel to nearest base
                GameObject closestBase = FindClosestBase(this.transform.parent.gameObject);
                ExecuteEvents.Execute<IUnitMessageHandler>(this.transform.parent.gameObject, null, (x, y) => x.OrderUnitToCoords(closestBase.transform.position));
                isBusy = true;
            }
        }
    }

    GameObject FindClosestBase(GameObject src)
    {
        Base[] bases = GameObject.FindObjectsOfType<Base>();
        Base closestBase = null;
        float closestBaseDist = -1;

        foreach (Base baseObj in bases)
        {
            float dist = Vector3.Distance(src.transform.position, baseObj.transform.position);

            if (closestBaseDist == -1 || dist < closestBaseDist)
            {
                closestBaseDist = dist;
                closestBase = baseObj;
            }
        }

        return closestBase.gameObject;
    }

    private IEnumerator CollectResource()
    {
        isBusy = true;
        numResources = 0;

        do
        {
            yield return new WaitForSeconds(collectingTime);
            ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(1));
            numResources++;
        } while (taskTargetObj != null && numResources < maxResources);

        isBusy = false;
        curSubroutine = SubRoutine.TRAVEL_TO_DEPOSIT;
    }
}