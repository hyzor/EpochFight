﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour {
    public GameObject selectedObj;
    public GameObject actionObj;
    public Vector3 actionCoordinates;
    private Color selectionColorCache;
    private GameObject selectedCanvasElement;

    private Renderer selectedObjRenderer;

    private Color selectedColorStart = Color.white;
    private Color selectedColorEnd = Color.black;
    private float duration = 1.0f;

    // Use this for initialization
    void Start () {
        Transform canvasResourceTextTrans = GameObject.Find("Canvas").transform.GetChild(1);
        selectedCanvasElement = canvasResourceTextTrans.gameObject;
    }

    // Update is called once per frame
    void Update () {

        if (selectedObj != null)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            selectedObjRenderer.material.color = Color.Lerp(selectedColorStart, selectedColorEnd, lerp);
        }

        // Listen for select (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast (ray, out hit, 100.0f))
            {
                Debug.Log("Left click hit " + hit.transform.name);
                GameObject selection = hit.transform.gameObject;

                if (selectedObj != null && selectedObj != selection)
                {
                    Deselect(selectedObj);
                }

                if (hit.transform.name == "Ground")
                    return;

                Select(hit.transform.gameObject);

                ExecuteEvents.Execute<IClickable>(selectedObj, null, (x, y) => x.OnLeftClick());
            }
        }

        // Listen for action (right mouse button)
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Right click on " + actionCoordinates);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("Right click hit " + hit.transform.name);
                actionObj = hit.transform.gameObject;
                actionCoordinates = hit.point;

                ExecuteEvents.Execute<IClickable>(actionObj, null, (x, y) => x.OnRightClick());
            }
        }
	}

    private void Select(GameObject obj)
    {
        if (selectedObj == obj)
        {
            Deselect(selectedObj);
            return;
        }

        selectedObj = obj;
        selectedObjRenderer = selectedObj.GetComponent<Renderer>();
        selectionColorCache = selectedObjRenderer.material.color;
        //renderer.material.color = Color.red;

        ExecuteEvents.Execute<ICanvasMessageHandler>(selectedCanvasElement, null, (x, y) => x.SetComponentText("Selected: " + selectedObj.name));
    }

    private void Deselect(GameObject obj)
    {
        selectedObjRenderer.material.color = selectionColorCache;
        selectedObj = null;
        selectedObjRenderer = null;

        ExecuteEvents.Execute<ICanvasMessageHandler>(selectedCanvasElement, null, (x, y) => x.SetComponentText("Selected: "));
    }
}
