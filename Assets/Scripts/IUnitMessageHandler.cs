using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUnitMessageHandler : IEventSystemHandler
{
    void OrderUnitToCoords(Vector3 coords);
    bool HasReachedDestination();
}
