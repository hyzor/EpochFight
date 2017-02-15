using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraScript : MonoBehaviour {
    public float pingPongDuration = 10.0f;
    public float zEndOffset;
    public float zStartOffset;

    public float yRotStart = -45.0f;
    public float yRotEnd = 20.0f;
    public float xRotationAngle = 45.0f;

    public float rotSmoothing = 2.0f;

    private float zStart;
    private float zEnd;

    // Use this for initialization
    void Start()
    {
        this.zStart = this.gameObject.transform.position.z + zStartOffset;
        this.zEnd = this.gameObject.transform.position.z + zEndOffset;
        //this.zEnd = zStart + 15.0f;

    }

    // Update is called once per frame
    void Update()
    {
        float pingPong = Mathf.PingPong(Time.time, pingPongDuration) / pingPongDuration;
        Transform trans = this.gameObject.transform;
        float zLerp = Mathf.Lerp(zStart, zEnd, pingPong);
        float rotYLerp = Mathf.Lerp(yRotStart, yRotEnd, pingPong);
        Vector3 newPos = new Vector3(trans.position.x, trans.position.y, zLerp);
        Quaternion targetRotQuat = Quaternion.Euler(xRotationAngle, rotYLerp, trans.rotation.z);
        Quaternion quatSlerp = Quaternion.Slerp(trans.rotation, targetRotQuat, Time.deltaTime * rotSmoothing);
        trans.position = newPos;
        trans.rotation = quatSlerp;
    }
}
