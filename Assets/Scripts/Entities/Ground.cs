using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour, IClickable {
    private MouseListener mouseListener;

	public void OnLeftClick(Vector3 point)
    {
        // Do nothing
    }

	public void OnRightClick(Vector3 point)
    {
		foreach(GameObject o in mouseListener.GetSelectedAlliedUnits()) {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.GOTO));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationCoords(point));
        }
    }

    // Use this for initialization
    void Start () {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
