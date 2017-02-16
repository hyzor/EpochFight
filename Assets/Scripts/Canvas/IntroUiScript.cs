using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroUiScript : MonoBehaviour {

    private Image tapToStartImg;
    private Color tapToStartImgColor;
    private float lerp;
    public float duration = 1.0f;

    private GameObject tapToStart;
    private GameObject epochFightLogo;

    private Vector3 camOrigPos;
    private Quaternion camOrigRot;
    private CameraScript camScript;

    // Use this for initialization
    void Start ()
    {
        tapToStart = GameObject.Find("TapToStart");
        epochFightLogo = GameObject.Find("EpochFightLogo");
        tapToStartImg = tapToStart.GetComponent<Image>();
        tapToStartImgColor = tapToStartImg.color;
        camOrigPos = Camera.main.transform.position;
        camOrigRot = Camera.main.transform.rotation;
        camScript = Camera.main.GetComponent<CameraScript>();
        camScript.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            Camera.main.transform.position = camOrigPos;
            Camera.main.transform.rotation = camOrigRot;
            camScript.enabled = true;
            Destroy(tapToStart);
            Destroy(epochFightLogo);
            Destroy(Camera.main.GetComponent<IntroCameraScript>());
            Destroy(this.gameObject);
            return;
        }

        float pingPong = Mathf.PingPong(Time.time, duration) / duration;
        lerp = Mathf.Lerp(1.0f, 0.0f, pingPong);

        Color color = tapToStartImg.color;
        color.a = lerp;
        tapToStartImg.color = color;
	}
}
