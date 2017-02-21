using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        this.gameObject.GetComponentInChildren<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
