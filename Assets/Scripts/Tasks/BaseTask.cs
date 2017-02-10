using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTask : MonoBehaviour
{
    public enum TaskType
    {
        IDLE = 0,
        COLLECT = 1,
        CARRY = 2,
        DEFEND = 3,
        ATTACK = 4
    }

    public delegate void OnFinished();

	public void SetFinishedListener(OnFinished cb)
    {
        Debug.Log("Task finished!");
	}

    public TaskType taskType;
}