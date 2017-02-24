using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchBetween : MonoBehaviour {

	public Vector3 origin;
	public Vector3 target;

	public void Start () {
	}

	public void Update () {
		transform.localScale = new Vector3(
			transform.localScale.x,
			transform.localScale.y,
			Vector3.Distance(origin,target));
		transform.position = (origin + target)/2;	
		transform.LookAt(target);
	}
}
