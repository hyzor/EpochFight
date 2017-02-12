using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ground : MonoBehaviour, IClickable {
    private MouseListener mouseListener;

    public void OnLeftClick()
    {
        // Do nothing
    }

    public void OnRightClick()
    {
        if (mouseListener.selectedObj != null && mouseListener.selectedObj.GetComponent<Enemy>() == null)
        {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(mouseListener.selectedObj, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.GOTO));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(mouseListener.selectedObj, null, (x, y) => x.SetTaskDestinationCoords(mouseListener.actionCoordinates));
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
