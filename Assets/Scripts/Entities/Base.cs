using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Base : MonoBehaviour, IClickable
{
    private MouseListener mouseListener;
    private Entity entity;

	// Use this for initialization
	void Start ()
    {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
        entity = gameObject.GetComponent<Entity>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (entity.deathTrigger && !entity.flaggedForRemoval)
        {
            entity.flaggedForRemoval = true;
        }
	}

	public void OnLeftClick(Vector3 point)
    {
        Debug.Log("Base left clicked!");
    }

	public void OnRightClick(Vector3 point)
    {
		GameObject[] selectedWorkers = mouseListener.GetSelectedAlliedUnits();

		foreach (GameObject o in selectedWorkers) {
            Worker workerComponent = o.GetComponent<Worker>();

            if (workerComponent.numResources > 0)
            {
                ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.COLLECT));
                ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationCoords(this.gameObject.transform.position));
                ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationObj(this.gameObject));
            }
        }

        Debug.Log("Base right clicked");
    }
}
