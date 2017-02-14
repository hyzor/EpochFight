using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IClickable {
    private MouseListener mouseListener;

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
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedUnit, null, (x, y) => x.SetTaskDestinationCoords(this.gameObject.transform.position));
            ExecuteEvents.Execute<ITaskManagerMessageHandler>(selectedUnit, null, (x, y) => x.SetTaskDestinationObj(this.gameObject));
            Debug.Log("Enemy right clicked!");
        }
    }

    // Use this for initialization
    void Start ()
    {
        mouseListener = GameObject.Find("MouseListener").GetComponent<MouseListener>();
    }

    // Update is called once per frame
    void Update ()
    {
    }
}
