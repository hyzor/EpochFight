using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTaskManager : MonoBehaviour, SetTaskMessageHandler {

	public BaseTask[] apas;

	private Queue tasksToDo = new Queue();
	private BaseTask curTask = null;

	public void Start () {
		
	}

	public void Update () {
		if (curTask == null) {
			curTask = (BaseTask)Instantiate(apas[0]);
			curTask.SetFinishedListener(OnTaskFinished);
		}
	}

	private void OnTaskFinished() {
		Debug.Log("task was finished");
	}

	public void SetTask() {
	}

	public void SetTaskOverrideAndClearAllOthers() {

	}		
}
