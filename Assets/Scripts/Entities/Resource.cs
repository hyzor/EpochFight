using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IClickable
{
    private MouseListener mouseListener;
    private Entity entity;

    public void OnLeftClick()
    {
        Debug.Log("Resource left clicked!");
    }

    public void OnRightClick()
    {
		foreach (GameObject o in mouseListener.GetSelectedAlliedWorkerUnits()) {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.COLLECT));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationCoords(this.gameObject.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationObj(this.gameObject));
            Debug.Log("Resource right clicked!");
        }
    }

	// Use this for initialization
	void Start () {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
        entity = GetComponent<Entity>();
    }
	
	// Update is called once per frame
	void Update () {
        if (entity.deathTrigger)
        {
            entity.isAlive = false;
            Destroy(this.gameObject);
        }
	}
}
