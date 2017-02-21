using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Patrol : MonoBehaviour {
    private NavMeshAgent navMesh;
    public List<Transform> waypoints;
    private int curWaypoint = 0;

	// Use this for initialization
	void Start ()
    {
        navMesh = this.gameObject.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void NextWaypoint()
    {
        if (waypoints.Count == 0)
            return;

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.RequestSetTask(BaseTask.TaskType.GOTO));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationCoords(waypoints[curWaypoint].position));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationObj(waypoints[curWaypoint].gameObject));

        curWaypoint = (curWaypoint + 1) % waypoints.Count;
    }
}
