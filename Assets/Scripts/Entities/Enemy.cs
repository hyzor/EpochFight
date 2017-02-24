using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IClickable {
    private MouseListener mouseListener;
    private Vector3 startPos;
    private Unit unit;

    private SphereCollider detectionRadiusSphere;

    private DetectionCollider detCol;
    public bool randomMovement = true;

    private Patrol patrol;
    private float timeSinceLastPatrol;
    bool pollNextPatrolWaypoint = false;
    private Entity entity;

	public void OnLeftClick(Vector3 point)
    {
        // Do nothing
    }

	public void OnRightClick(Vector3 point)
    {
        // Order selected unit to attack this enemy
        GameObject[] selectedUnits = mouseListener.GetSelectedAlliedUnits();

		foreach (GameObject o in selectedUnits) {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.ATTACK));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationCoords(gameObject.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(o, null, (x, y) => x.SetTaskDestinationObj(gameObject));
            Debug.Log("Enemy right clicked!");
        }
    }

    // Use this for initialization
    void Start ()
    {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
        detectionRadiusSphere = gameObject.GetComponentInChildren<SphereCollider>();
        unit = gameObject.GetComponent<Unit>();
        startPos = gameObject.transform.position;
        detCol = gameObject.GetComponentInChildren<DetectionCollider>();
        patrol = gameObject.GetComponent<Patrol>();
        entity = this.gameObject.GetComponent<Entity>();
    }

    void MoveRandomly()
    {
        Vector3 randomDir = (UnityEngine.Random.insideUnitSphere * detectionRadiusSphere.radius) + startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, detectionRadiusSphere.radius, 1);
        Vector3 finalPos = hit.position;

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(gameObject, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.GOTO));
        ExecuteEvents.Execute<ITaskManagerMessageHandler>(gameObject, null, (x, y) => x.SetTaskDestinationCoords(finalPos));
    }

    // Update is called once per frame
    void Update ()
    {
        if (!entity.isAlive)
            return;

        // Are we idle?
        if (!unit.HasTaskAssigned())
        {
            if (patrol != null)
            {
                // Start taking time since we last patrolled
                if (!pollNextPatrolWaypoint)
                {
                    timeSinceLastPatrol = Time.time;
                    pollNextPatrolWaypoint = true;
                }

                // If we have waited long enough, start patrolling to the next waypoint
                if (Time.time - timeSinceLastPatrol >= patrol.patrolWaitTime)
                {
                    patrol.NextWaypoint();
                    pollNextPatrolWaypoint = false;
                }
            }
            else if (randomMovement)
                MoveRandomly();
        }

        // If we find a nearby attackable target while busy with a GOTO task, we override it with an ATTACK task
        else if (unit.HasTaskAssigned() && unit.GetCurTaskType() == BaseTask.TaskType.GOTO)
        {
            if (unit.FindTargetAndAttack())
                Debug.Log(this.name + " attacking!");
        }
    }
}
