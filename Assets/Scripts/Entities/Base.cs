using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, IClickable
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnLeftClick()
    {
        Debug.Log("Base left clicked!");
    }

    public void OnRightClick()
    {
        Debug.Log("Base right clicked");
    }
}
