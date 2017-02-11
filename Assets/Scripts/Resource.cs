using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IClickable
{
    private MouseListener mouseListener;

    public void OnLeftClick()
    {
        Debug.Log("Resource left clicked!");
    }

    public void OnRightClick()
    {
        GameObject selectedWorker = GetSelectedWorker();

        if (selectedWorker != null)
        {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedWorker, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.COLLECT));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedWorker, null, (x, y) => x.SetTaskDestinationCoords(this.gameObject.transform.position));
            Debug.Log("Resource right clicked!");
        }
    }

    GameObject GetSelectedWorker()
    {        
        if (mouseListener.selectedObj != null && mouseListener.selectedObj.name == "Worker")
        {
            return mouseListener.selectedObj;
        }

        return null;
    }

	// Use this for initialization
	void Start () {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
