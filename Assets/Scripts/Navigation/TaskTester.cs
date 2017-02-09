using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskTester : MonoBehaviour {

	public GameObject obj;

	public void Start () {
		
	}

	public void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			ExecuteEvents.Execute<SetTaskMessageHandler>(obj, null, (x,y)=>x.SetTask());
		}
	}
}
