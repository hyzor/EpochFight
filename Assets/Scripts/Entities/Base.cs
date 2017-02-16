using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Base : MonoBehaviour, IClickable
{
    private MouseListener mouseListener;

	// Use this for initialization
	void Start ()
    {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnLeftClick()
    {
        Debug.Log("Base left clicked!");
    }

    public void OnRightClick()
    {
        GameObject selectedWorker = mouseListener.GetSelectedAlliedWorker();
        Worker workerComponent = selectedWorker.GetComponent<Worker>();

        if (selectedWorker != null && workerComponent.numResources > 0)
        {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedWorker, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.COLLECT));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedWorker, null, (x, y) => x.SetTaskDestinationCoords(this.gameObject.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedWorker, null, (x, y) => x.SetTaskDestinationObj(this.gameObject));
        }

        Debug.Log("Base right clicked");
    }
}
