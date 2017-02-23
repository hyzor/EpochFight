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

    public void OnLeftClick()
    {
        // Do nothing
    }

    public void OnRightClick()
    {
        // Order selected unit to attack this enemy
        GameObject selectedUnit = mouseListener.GetSelectedAlliedUnit();

        if (selectedUnit != null)
        {
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedUnit, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.ATTACK));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedUnit, null, (x, y) => x.SetTaskDestinationCoords(gameObject.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedUnit, null, (x, y) => x.SetTaskDestinationObj(gameObject));
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

    private void OrderAttackOn(GameObject obj)
    {
        if (obj == null || !obj.GetComponent<Entity>().isAlive)
            return;

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.RequestSetTask(BaseTask.TaskType.ATTACK));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationCoords(obj.transform.position));

        ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y)
            => x.SetTaskDestinationObj(obj));
    }

    private bool FindTargetAndAttack()
    {
        GameObject targetObj = null;

        // Is there an enemy unit in sight? (First prio)
        if (detCol.objectsInSight.ContainsKey(DetectionCollider.ObjTypes.UNIT))
        {
            targetObj = detCol.objectsInSight[DetectionCollider.ObjTypes.UNIT];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(DetectionCollider.ObjTypes.UNIT);
                return false;
            }
        }

        // Is there an enemy base in sight? (Second prio)
        else if (detCol.objectsInSight.ContainsKey(DetectionCollider.ObjTypes.BASE))
        {
            targetObj = detCol.objectsInSight[DetectionCollider.ObjTypes.BASE];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(DetectionCollider.ObjTypes.BASE);
                return false;
            }
        }

        // Is there an enemy building in sight? (Third prio)
        else if (detCol.objectsInSight.ContainsKey(DetectionCollider.ObjTypes.BUILDING))
        {
            targetObj = detCol.objectsInSight[DetectionCollider.ObjTypes.BUILDING];

            if (targetObj == null || !targetObj.GetComponent<Entity>().isAlive)
            {
                detCol.objectsInSight.Remove(DetectionCollider.ObjTypes.BUILDING);
                return false;
            }
        }

        if (targetObj != null)
        {
            OrderAttackOn(targetObj);
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!entity.isAlive)
            return;

        // Are we idle?
        if (!unit.HasTaskAssigned())
        {
            // Try to find an attackable target
            if (FindTargetAndAttack())
                Debug.Log(this.name + " attacking!");
            else if (patrol != null)
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
            if (FindTargetAndAttack())
                Debug.Log(this.name + " attacking!");
        }
    }
}
