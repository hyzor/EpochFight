using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public float moveSpeed = 1.0f;
    public float turnSpeed = 1.0f;

	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(new Vector3(0.0f, moveSpeed * Time.deltaTime, 0.0f));
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(new Vector3(0.0f, -(moveSpeed * Time.deltaTime), 0.0f));
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(new Vector3(-(moveSpeed * Time.deltaTime), 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(new Vector3(0.0f, 0.0f, moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(new Vector3(0.0f, 0.0f, -(moveSpeed * Time.deltaTime)));
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(new Vector3(0.0f, -(turnSpeed * Time.deltaTime), 0.0f));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(new Vector3(0.0f, turnSpeed * Time.deltaTime, 0.0f));
        }
    }
}
