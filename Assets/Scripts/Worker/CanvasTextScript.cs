using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTextScript : MonoBehaviour {

    private Text text;

    private int resourceNum;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.text = "Test";
        resourceNum = 0;
	}
	
	// Update is called once per frame
	void Update () {
        text.text = "Resources: " + resourceNum;
	}

    public void IncrementResource(int amount)
    {
        resourceNum += amount;
    }

    public void DecrementResource(int amount)
    {
        resourceNum -= amount;
    }
}
