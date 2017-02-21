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
    private Worker worker;

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
        Transform canvasResourceTextTrans = GameObject.Find("Canvas").transform.FindChild("ResourceText");
        worker = this.gameObject.transform.parent.GetComponent<Worker>();
        resourceCanvasElement = canvasResourceTextTrans.gameObject;

        if (taskTargetObj.GetComponent<Base>() != null)
            curSubroutine = SubRoutine.TRAVEL_TO_DEPOSIT;
        else
            curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
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
            ExecuteEvents.Execute<ICanvasMessageHandler>(resourceCanvasElement, null, (x, y) => x.IncrementComponentValue(worker.numResources));
            worker.numResources = 0;
            isBusy = false;

            if (taskTargetObj != null && taskTargetObj.GetComponent<Base>() == null)
            {
                curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
            }
            else
            {
                // Find another resource that is close
                GameObject closestRes = FindClosestResource(taskCoords);

                // If we have found a resource close to the depleted one, continue collecting on this
                if (closestRes != null)
                {
                    taskCoords = closestRes.transform.position;
                    taskTargetObj = closestRes;
                    curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
                }

                // We haven't found any resource in the vicinity, destroy the task
                else
                {
                    Destroy(this.gameObject);
                }
            }
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

    GameObject FindClosestResource(Vector3 coords)
    {
        Resource[] resources = GameObject.FindObjectsOfType<Resource>();
        Resource closestRes = null;
        float closesResDist = float.MaxValue;

        foreach(Resource resObj in resources)
        {
            float dist = Vector3.Distance(coords, resObj.transform.position);

            if (dist < closesResDist)
            {
                closesResDist = dist;
                closestRes = resObj;
            }
        }

        if (closestRes != null)
            return closestRes.gameObject;

        return null;
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

        if (closestBase != null)
            return closestBase.gameObject;

        return null;
    }

    private IEnumerator CollectResource()
    {
        isBusy = true;

        Entity curResEntity = null;

        if (taskTargetObj != null)
            curResEntity = taskTargetObj.GetComponent<Entity>();

        while (taskTargetObj != null && worker.numResources < worker.resourceCapacity)
        {
            // Collect as normal
            if (worker.resPerCollect <= curResEntity.curHealth && worker.numResources + worker.resPerCollect <= worker.resourceCapacity)
            {
                yield return new WaitForSeconds(collectingTime);

                // Double-check resource health after waiting
                if (curResEntity.curHealth > 0)
                {
                    ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(worker.resPerCollect, this.transform.parent.gameObject));
                    worker.numResources += worker.resPerCollect;
                }
            }

            // We are about to reach our capacity, so fill the worker with the remaining number of resources
            else if (worker.resPerCollect <= curResEntity.curHealth && worker.numResources + worker.resPerCollect > worker.resourceCapacity)
            {
                yield return new WaitForSeconds(collectingTime);

                // Double-check resource health after waiting
                if (curResEntity.curHealth > 0)
                {
                    int resToFill = worker.resourceCapacity - worker.numResources;
                    ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(resToFill, this.transform.parent.gameObject));
                    worker.numResources += resToFill;
                }
            }

            // The resource contains less resources than what we can collect, so collect the remaining amount
            else
            {
                yield return new WaitForSeconds(collectingTime);

                // Double-check resource health after waiting
                if (curResEntity.curHealth > 0)
                {
                    int resToFill = curResEntity.curHealth;
                    ExecuteEvents.Execute<IEntityMessageHandler>(taskTargetObj, null, (x, y) => x.ReceiveDamage(resToFill, this.transform.parent.gameObject));
                    worker.numResources += resToFill;
                }
            }
        }

        // We still have the capacity to collect more, find closest resource
        if (worker.numResources < worker.resourceCapacity)
        {
            GameObject closestRes = FindClosestResource(taskCoords);

            if (closestRes != null)
            {
                taskCoords = closestRes.transform.position;
                taskTargetObj = closestRes;
                curSubroutine = SubRoutine.TRAVEL_TO_COLLECT;
                isBusy = false;
                yield return null;
            }
            else
            {
                isBusy = false;
                curSubroutine = SubRoutine.TRAVEL_TO_DEPOSIT;
                yield return null;
            }
        }
        else
        {
            isBusy = false;
            curSubroutine = SubRoutine.TRAVEL_TO_DEPOSIT;
            yield return null;
        }
    }
}