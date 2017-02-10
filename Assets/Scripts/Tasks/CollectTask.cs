using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTask : BaseTask
{
    private int numResources;
    private int maxResources;
    private int collectingTime;
    private TaskState curState;

    private enum TaskState
    {
        COLLECT = 0,
        CARRY = 1
    }

    public void Start()
    {
        maxResources = 5;
        collectingTime = 1;
        numResources = 0;
    }

    public void Update()
    {
        // Check if unit has reached the location first

        // Then start collection routine
        if (curState == TaskState.COLLECT)
            StartCoroutine(CollectResource());
    }

    private IEnumerator CollectResource()
    {
        numResources = 0;

        do
        {
            yield return new WaitForSeconds(collectingTime);
            numResources++;
        } while (numResources < maxResources);
    }
}