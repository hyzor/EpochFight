using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resource : MonoBehaviour, IClickable
{
    GameObject workerObj = null;

    public void OnLeftClick()
    {
        Debug.Log("Resource left clicked!");
    }

    public void OnRightClick()
    {
        GetSelectedWorker();
        ExecuteEvents.Execute<ITaskMessageHandler>(workerObj, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.COLLECT));
        Debug.Log("Resource right clicked!");
    }

    void GetSelectedWorker()
    {
        workerObj = GameObject.Find("Worker").transform.gameObject;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
