using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseTask : MonoBehaviour, ITaskMessageHandler
{
    public enum TaskType
    {
        IDLE = 0,
        GOTO = 1,
        COLLECT = 2,
        ATTACK = 3
    }

    public void SetTaskTargetCoordinates(Vector3 coords)
    {
        this.taskCoords = coords;
    }

    public void SetTaskTargetObject(GameObject target)
    {
        this.taskTargetObj = target;
    }

    public void SetTaskIsBusy(bool isBusy)
    {
        this.isBusy = isBusy;
    }

    public virtual void OnDestReached()
    {
        isBusy = false;
    }

    public TaskType taskType;
    public Vector3 taskCoords;
    public GameObject taskTargetObj;
    public bool isBusy = false;
    public bool completed = false;
    public bool isActive = false;
}