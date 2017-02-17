using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IClickable {
    private MouseListener mouseListener;
    private NavMeshAgent navMesh;
    public float attackDist = 25.0f;
    private Vector3 startPos;
    private Unit unit;

    private SphereCollider detectionRadiusSphere;

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
        navMesh = gameObject.GetComponent<NavMeshAgent>();
        detectionRadiusSphere = gameObject.GetComponentInChildren<SphereCollider>();
        unit = gameObject.GetComponent<Unit>();
        startPos = gameObject.transform.position;
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

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObj = other.gameObject;

        if (otherObj.GetComponent<Unit>() != null || otherObj.GetComponent<Base>() != null)
        {
            if (otherObj.GetComponent<Enemy>() == null)
            {
                if (unit.curState != Unit.State.ATTACKING)
                {
                    ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.RequestSetTask(BaseTask.TaskType.ATTACK));
                    ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.SetTaskDestinationCoords(otherObj.transform.position));
                    ExecuteEvents.Execute<ITaskManagerMessageHandler>(this.gameObject, null, (x, y) => x.SetTaskDestinationObj(otherObj));
                }
            }
        }

        Debug.Log("Enemy trigger enter!");
    }

    // Update is called once per frame
    void Update ()
    {
        if (!unit.HasTaskAssigned())
        {
            MoveRandomly();
        }
    }
}
