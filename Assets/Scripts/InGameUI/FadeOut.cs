using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

	public float initAlpha = 1;
	public float seconds = 3;
	public bool fade = false;

	private float alpha;

	public void Start () {
		alpha = initAlpha;
	}

	public void Update () {
		if (fade) {
			alpha -= Time.deltaTime / seconds;
		}
		Color c = GetComponent<MeshRenderer>().material.color;
		GetComponent<MeshRenderer>().material.color = new Color(c.r, c.g, c.b, alpha);
	}

	public void StartFade() {
		fade = true;
		alpha = initAlpha;
	}

	public void Reset() {
		fade = false;
		alpha = initAlpha;
	}
}
