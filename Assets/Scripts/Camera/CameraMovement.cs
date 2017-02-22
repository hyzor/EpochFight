using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public float perspectiveZoomSpeed = 0.5f;    
	public float movementSpeed = 1.5f;
	public float touchMovementThreshHold = 1.5f;


	// Update is called once per frame
	void Update () {

		// Detect swipe.
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {

			// Store current rotation.
			Quaternion rot = this.transform.rotation;
			this.transform.rotation = new Quaternion (0.0f, 0.0f, 0.0f, 0.0f);

			Vector2 touchDeltaPosition = Input.GetTouch (0).deltaPosition;

			// Make sure the user swipe and not click on the screen.
			if (touchDeltaPosition.x >= touchMovementThreshHold || touchDeltaPosition.x <= -touchMovementThreshHold || 
				touchDeltaPosition.y >= touchMovementThreshHold || touchDeltaPosition.y <= -touchMovementThreshHold) {

				this.transform.Translate (touchDeltaPosition.x * movementSpeed, 0, touchDeltaPosition.y * movementSpeed);
			}

			// Restore rotation.
			this.transform.rotation = rot;
		}

		// Detect zoom in/out.
		else if (Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Move camera backward and keep current rotation.
			this.transform.Translate (-Vector3.forward * deltaMagnitudeDiff);
		}
	}
}
