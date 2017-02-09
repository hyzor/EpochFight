using UnityEngine.EventSystems;

public interface SetTaskMessageHandler : IEventSystemHandler {
	void SetTask();
	void SetTaskOverrideAndClearAllOthers();
}
