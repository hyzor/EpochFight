using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour {
    public GameObject selectedObj;
    private Entity selectedEntity;
    public GameObject actionObj;
    public Vector3 actionCoordinates;
    private Color selectionColorCache;
    private GameObject selectedCanvasElement;
    public float maxRaycastDist = 1000.0f;

    private Renderer selectedObjRenderer;

    private Color selectedColorStart = Color.white;
    private Color selectedColorEnd = Color.black;
    private float duration = 1.0f;

    // Use this for initialization
    void Start ()
    {
        GameObject canvasOverlayObj = GameObject.Find("Canvas_Overlay");

        if (canvasOverlayObj != null)
        {
            selectedCanvasElement = canvasOverlayObj.transform.GetChild(1).gameObject;
        }
    }

    // Update is called once per frame
    void Update ()
    {

        if (selectedEntity != null)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;

            foreach(Renderer renderer in selectedEntity.entityRenderers)
            {
                renderer.material.color = Color.Lerp(selectedColorStart, selectedColorEnd, lerp);
            }
        }

        // Listen for select (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast (ray, out hit, maxRaycastDist))
            {
                Debug.Log("Left click hit " + hit.transform.name);
                GameObject selection = hit.transform.gameObject;

                if (selectedObj != null && selectedObj != selection)
                {
                    Deselect(selectedObj);
                }

                // Ingore ground (TODO: also ignore environment)
                if (hit.transform.gameObject.GetComponent<Ground>() != null)
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

            if (Physics.Raycast(ray, out hit, maxRaycastDist))
            {
                Debug.Log("Right click hit " + hit.transform.name);
                actionObj = hit.transform.gameObject;
                actionCoordinates = hit.point;

                ExecuteEvents.Execute<IClickable>(actionObj, null, (x, y) => x.OnRightClick());
            }
        }
	}

    public GameObject GetSelectedAlliedWorker()
    {
        if (selectedObj != null && selectedObj.GetComponent<Worker>() != null)
        {
            if (selectedObj.GetComponent<Enemy>() == null)
            {
                return selectedObj;
            }
        }

        return null;
    }

    public GameObject GetSelectedAlliedUnit()
    {
        if (selectedObj != null)
        {
            if (selectedObj.GetComponent<Unit>() != null && selectedObj.GetComponent<Enemy>() == null)
            {
                return selectedObj;
            }
        }

        return null;
    }

    private void Select(GameObject obj)
    {
        if (selectedObj == obj)
        {
            Deselect(selectedObj);
            return;
        }

        selectedObj = obj;
        selectedEntity = selectedObj.GetComponent<Entity>();

        if (selectedCanvasElement != null)
            ExecuteEvents.Execute<ICanvasMessageHandler>(selectedCanvasElement, null, (x, y) => x.SetComponentText("Selected: " + selectedObj.name));
    }

    private void Deselect(GameObject obj)
    {
        for (int i = 0; i < selectedEntity.entityRenderers.Count; ++i)
        {
            selectedEntity.entityRenderers[i].material.color = Color.white;
        }

        selectedObj = null;
        selectedEntity = null;
        selectedObjRenderer = null;

        if (selectedCanvasElement != null)
            ExecuteEvents.Execute<ICanvasMessageHandler>(selectedCanvasElement, null, (x, y) => x.SetComponentText("Selected: "));
    }
}
