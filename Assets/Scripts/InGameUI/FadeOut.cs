using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

	public float alpha = 1;
	public float seconds = 3;
	private bool fade = false;

	public void Start () {
		
	}

	public void Update () {
		alpha -= Time.deltaTime / seconds;
		Color c = GetComponent<MeshRenderer>().material.color;
		GetComponent<MeshRenderer>().material.color = new Color(c.r, c.g, c.b, alpha);
	}

	public void StartFade() {
		fade = true;
	}

	public void Reset() {
		fade = false;
		alpha = 1;
	}
}
