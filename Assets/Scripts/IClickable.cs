using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IClickable : IEventSystemHandler
{
	void OnLeftClick(Vector3 point);
    void OnRightClick(Vector3 point);
}
