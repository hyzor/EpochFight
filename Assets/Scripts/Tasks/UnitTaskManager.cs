using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour, ITaskMessageHandler {

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

	public void Start ()
    {
        curTask = null;
	}

	public void Update ()
    {
        // If mouse is clicked and this unit is selected

        // If enemy is close
        //OnEnemyClose();

        // Execute task
        //curTask.execute();
	}

	private void OnTaskFinished()
    {
        
        Debug.Log("Task finished!");
	}

    public void OnEnemyClose()
    {
        int index = FindTask(BaseTask.TaskType.DEFEND);

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

        return -1;
    }

    public void RequestSetTask(BaseTask.TaskType taskType)
    {
        ForceDestroyCurrentTask();
        int index = FindTask(taskType);

        if (index != -1)
        {
            InstantiateCurTaskAndSetParent(index);
        }
    }

    private void InstantiateCurTaskAndSetParent(int newTaskIdx)
    {
        curTask = Instantiate(tasksAvailable[newTaskIdx]);
        curTask.transform.parent = this.gameObject.transform;
    }

    private void ForceDestroyCurrentTask()
    {
        Destroy(curTask);
        curTask = null;
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
            ForceDestroyCurrentTask();
            curTask = Instantiate(tasksAvailable[index]);
        }
    }
}
