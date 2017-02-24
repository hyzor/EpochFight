using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour {
	public float maxRaycastDist = 1000.0f;

	private List<Entity> selectedEntities = new List<Entity>();
    private Color selectionColorCache;
    private GameObject selectedCanvasElement;
	private GameObject selectionSphere;
    private Color selectedColorStart = Color.white;
    private Color selectedColorEnd = Color.black;
    private float duration = 1.0f;

    // Use this for initialization
    void Start ()
    {
        GameObject canvasOverlayObj = GameObject.Find("Canvas_Overlay");
		selectionSphere = transform.Find("SelectionSize").gameObject;

        if (canvasOverlayObj != null) {
            selectedCanvasElement = canvasOverlayObj.transform.GetChild(1).gameObject;
        }
    }

	private void SelectUnitsAtClick(Vector3 point) {
		DeselectAll();

		Collider[] hits = Physics.OverlapSphere(point, selectionSphere.GetComponent<SphereCollider>().radius);
		foreach (Collider c in hits) {
			if (c.gameObject.GetComponent<Entity>() != null && c.gameObject.GetComponent<Enemy>() == null && c.gameObject.GetComponent<Unit>() != null) {
				Select(c.gameObject.GetComponent<Entity>());
			}
		}

		if (selectedCanvasElement != null) {
			ExecuteEvents.Execute<ICanvasMessageHandler>(selectedCanvasElement, null, 
				(x, y) => x.SetComponentText ("Selected: " + selectedEntities.Count.ToString()));
		}
	}

    // Update is called once per frame
    void Update ()
    {
		// remove destroyed objects!
		selectedEntities.RemoveAll(o => o == null);

		// flash selected units
		foreach (Entity e in selectedEntities) {
			float lerp = Mathf.PingPong (Time.time, duration) / duration;

			foreach (Renderer renderer in e.entityRenderers) {
                if (renderer != null)
				    renderer.material.color = Color.Lerp (selectedColorStart, selectedColorEnd, lerp);
			}
		}

        if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, maxRaycastDist)) {
				if (selectedEntities.Count > 0) {
					// TODO we are still calling it "right click" 
					ExecuteEvents.Execute<IClickable>(hit.transform.gameObject, null, (x, y) => x.OnRightClick(hit.point));
					DeselectAll();
				} else {
					SelectUnitsAtClick (hit.point);
					Debug.Log ("Left click hit " + hit.transform.name);
				}
			}
        }
	}

    private void Select(Entity obj)
    {
		selectedEntities.Add(obj);
    }

	private void DeselectAll() {
		foreach (Entity e in selectedEntities) {
			for (int i = 0; i < e.entityRenderers.Count; ++i)
			{
				e.entityRenderers[i].material.color = Color.white;
			}
		}
		selectedEntities.Clear();
		ExecuteEvents.Execute<ICanvasMessageHandler> (selectedCanvasElement, null, (x, y) => x.SetComponentText ("Selected: "));
	}

    private void Deselect(Entity obj)
    {
		foreach (Entity e in selectedEntities) {
			if (e == obj) {
				for (int i = 0; i < e.entityRenderers.Count; ++i)
				{
					e.entityRenderers[i].material.color = Color.white;
				}
			}
		}

		if (selectedCanvasElement != null) {
			ExecuteEvents.Execute<ICanvasMessageHandler> (selectedCanvasElement, null, (x, y) => x.SetComponentText ("Selected: "));
		}
    }


	public GameObject[] GetSelectedAlliedWorkerUnits() {
		List<GameObject> ret = new List<GameObject>();
		foreach (Entity e in selectedEntities) {
			if (e.gameObject.GetComponent<Worker>() != null && e.gameObject.GetComponent<Enemy>() == null) {
				ret.Add(e.gameObject);
			}
		}
		return ret.ToArray();
	}

	public GameObject[] GetSelectedAlliedUnits()
	{
		List<GameObject> ret = new List<GameObject>();
		foreach (Entity e in selectedEntities) {
			if (e.gameObject.GetComponent<Unit>() != null && e.gameObject.GetComponent<Enemy>() == null) {
				ret.Add(e.gameObject);
			}
		}
		return ret.ToArray();
	}
}
