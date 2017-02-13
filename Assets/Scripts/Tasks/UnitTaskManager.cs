using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitTaskManager : MonoBehaviour, ITaskManagerMessageHandler {

    public struct Task
    {
        public int taskId;
        public Vector3 worldPos;

        public Task(int taskId, Vector3 worldPos)
        {
            this.taskId = taskId;
            this.worldPos = worldPos;
        }
    }

    public BaseTask[] tasksAvailable;
    private BaseTask curTask;

    private TextMesh unitStatusText;

    public Vector3 taskTargetCoordinates;
    public GameObject taskTargetObject;

	public void Start ()
    {
        curTask = null;
        unitStatusText = gameObject.GetComponentInChildren<TextMesh>();
    }

    public void Update ()
    {
        unitStatusText.transform.rotation = Camera.main.transform.rotation;

        if (curTask != null)
        {
            if (curTask.completed)
                PrepareForNextTask();
        }

        // If enemy is close
        //OnEnemyClose();
    }

    private void OnTaskFinished()
    {
        Debug.Log("Task finished!");
	}

    public void OnEnemyClose()
    {
        int index = FindTask(BaseTask.TaskType.ATTACK);

        if (index != -1)
        {
            Destroy(curTask);
            InstantiateCurTaskAndSetParent(index);
        }
    }

    private int FindTask(BaseTask.TaskType taskType)
    {
        for (int i = 0; i < tasksAvailable.Length; ++i)
        {
            BaseTask.TaskType taskTypeFound = tasksAvailable[i].taskType;

            if (taskTypeFound == taskType)
            {
                return i;
            }
        }

        Debug.LogWarning("Task " + taskType + " was not found for " + gameObject.name + "!");
        return -1;
    }

    public void RequestSetTask(BaseTask.TaskType taskType)
    {
        Debug.Log("Task of type " + taskType + " requested for unit " + gameObject.name);

        PrepareForNextTask();
        int index = FindTask(taskType);

        if (index != -1)
        {
            InstantiateCurTaskAndSetParent(index);
        }
    }

    public void SetTaskTargetCoordinates(Vector3 coords)
    {
        taskTargetCoordinates = coords;
    }

    public void SetTaskTargetObject(GameObject obj)
    {
        taskTargetObject = obj;
    }

    public void OnDestReached()
    {
        curTask.OnDestReached();
    }

    public bool TaskIsActive()
    {
        if (curTask != null)
            return curTask.isBusy;

        return false;
    }

    private void PrepareForNextTask()
    {
        taskTargetCoordinates = Vector3.zero;
        taskTargetObject = null;

        if (curTask)
        {
            Destroy(curTask.gameObject);
            Destroy(curTask);
            curTask = null;
        }
    }

    private void InstantiateCurTaskAndSetParent(int newTaskIdx)
    {
        curTask = Instantiate(tasksAvailable[newTaskIdx]);
        curTask.transform.parent = this.gameObject.transform;
    }

    public void AbortCurrentTask()
    {
        PrepareForNextTask();
    }

    public BaseTask GetCurrentTask()
    {
        return curTask;
    }

	public void OverrideWithTask(BaseTask.TaskType taskType)
    {
        int index = FindTask(taskType);

        if (index != -1)
        {
            PrepareForNextTask();
            curTask = Instantiate(tasksAvailable[index]);
        }
    }

    public void SetTaskDestinationCoords(Vector3 coords)
    {
        ExecuteEvents.Execute<ITaskMessageHandler>(curTask.gameObject, null, (x, y) => x.SetTaskTargetCoordinates(coords));
    }

    public void SetTaskDestinationObj(GameObject obj)
    {
        ExecuteEvents.Execute<ITaskMessageHandler>(curTask.gameObject, null, (x, y) => x.SetTaskTargetObject(obj));
    }
}
