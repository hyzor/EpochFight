using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTask : MonoBehaviour {
	public delegate void OnFinished();

	public void SetFinishedListener(OnFinished cb) {

	}
}


public class ApaTask : BaseTask {
	public int numBananas;

	public void Update() {
		Debug.Log("hej!");
	}
}