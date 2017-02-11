using UnityEngine.EventSystems;
using UnityEngine;

public interface ITaskMessageHandler : IEventSystemHandler
{
    void SetTaskTargetCoordinates(Vector3 coords);
    void SetTaskTargetObject(GameObject target);
}

public interface ITaskManagerMessageHandler : IEventSystemHandler
{
    void RequestSetTask(BaseTask.TaskType taskType);
    void OverrideWithTask(BaseTask.TaskType taskType);
    void SetTaskDestinationCoords(Vector3 coords);
    void SetTaskDestinationObj(GameObject obj);
}
