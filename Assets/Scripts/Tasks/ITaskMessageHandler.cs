using UnityEngine.EventSystems;
using UnityEngine;

public interface ITaskMessageHandler : IEventSystemHandler {
    void RequestSetTask(BaseTask.TaskType taskType);
	void OverrideWithTask(BaseTask.TaskType taskType);
}
