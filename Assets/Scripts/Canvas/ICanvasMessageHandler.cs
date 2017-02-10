using UnityEngine.EventSystems;

public interface ICanvasMessageHandler : IEventSystemHandler
{
    void SetComponentText(string text);
    void IncrementComponentValue(int value);
}
