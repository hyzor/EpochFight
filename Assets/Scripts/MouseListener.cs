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

	private StretchBetween directionMarker;
	private FadeOut selectionMarker;

    // Use this for initialization
    void Start ()
    {
        GameObject canvasOverlayObj = GameObject.Find("Canvas_Overlay");
		selectionSphere = transform.Find("SelectionSize").gameObject;
		selectionMarker = transform.Find("SelectionMarker").gameObject.GetComponent<FadeOut>();

        if (canvasOverlayObj != null) {
            selectedCanvasElement = canvasOverlayObj.transform.GetChild(1).gameObject;
        }

		directionMarker = transform.Find("DirectionMarker").GetComponent<StretchBetween>();
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

	private Vector3 FindCenterPoint(GameObject[] gos) {
		if (gos.Length == 0) {
			return Vector3.zero;
		}
		if (gos.Length == 1) {
			return gos [0].transform.position;
		}
		Bounds bounds = new Bounds(gos[0].transform.position, Vector3.zero);
		for (int i = 1; i < gos.Length; i++) {
			bounds.Encapsulate(gos[i].transform.position); 
		}
		return bounds.center;
	}

    void Update() {
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
				} else {
					SelectUnitsAtClick(hit.point);
					selectionMarker.StartFade();
					selectionMarker.gameObject.transform.position = hit.point;
					if (selectedEntities.Count > 1) {
						directionMarker.gameObject.GetComponent<MeshRenderer> ().enabled = true;
						directionMarker.target = hit.point;
						directionMarker.gameObject.GetComponent<FadeOut> ().Reset ();
					}
				}
			}
        }
		if (Input.GetMouseButtonUp(0)) {
			if (selectedEntities.Count > 0) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, maxRaycastDist)) {
					// TODO we are still calling it "right click" 
					ExecuteEvents.Execute<IClickable> (hit.transform.gameObject, null, (x, y) => x.OnRightClick (hit.point));
					DeselectAll ();
					//directionMarker.gameObject.GetComponent<MeshRenderer>().enabled = false;
					directionMarker.gameObject.GetComponent<FadeOut> ().StartFade ();
				}
			}
		}

		if (selectedEntities.Count > 0) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit, maxRaycastDist)) {
				directionMarker.target = hit.point;
				directionMarker.origin = FindCenterPoint(selectedEntities.ConvertAll(o=>o.gameObject).ToArray());
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
