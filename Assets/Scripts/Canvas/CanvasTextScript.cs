using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTextScript : MonoBehaviour, ICanvasMessageHandler {

    public Text uiText;
    public string text = "Placeholder";
    public int displayValue = 0;
    bool hasValue = false;

	// Use this for initialization
	void Start () {
        uiText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (hasValue)
            uiText.text = text + displayValue;
        else
            uiText.text = text;
	}

    public void SetComponentText(string text)
    {
        this.text = text;
    }

    public void IncrementComponentValue(int value)
    {
        displayValue += value;
    }
}
