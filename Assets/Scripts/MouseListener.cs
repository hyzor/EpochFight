using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour {
    public GameObject selectedObj;
    public GameObject actionObj;
    public Vector3 actionCoordinates;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Listen for select (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast (ray, out hit, 100.0f))
            {
                Debug.Log("Selection ray hit " + hit.transform.name);
                selectedObj = hit.transform.gameObject;

                ExecuteEvents.Execute<IClickable>(selectedObj, null, (x, y) => x.OnLeftClick());
            }
        }

        // Listen for action (right mouse button)
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            actionCoordinates = ray.GetPoint(100.0f);
            Debug.Log(actionCoordinates);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("Action ray hit " + hit.transform.name);
                actionObj = hit.transform.gameObject;

                ExecuteEvents.Execute<IClickable>(selectedObj, null, (x, y) => x.OnRightClick());
            }
        }
	}

    private GameObject GetClickedObject()
    {
        PointerEventData pEv = new PointerEventData(EventSystem.current);
        pEv.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pEv, hits);

        foreach (RaycastResult result in hits)
        {
            GameObject obj = result.gameObject;

            if (obj)
                return obj;
        }

        return null;
    }
}
