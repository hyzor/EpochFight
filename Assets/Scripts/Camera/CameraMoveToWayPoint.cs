using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveToWayPoint : MonoBehaviour {

	/// <summary>
	/// 
	/// </summary>
	public List<GameObject> waypointList = new List<GameObject>();

	/// <summary>
	/// The movement speed.
	/// </summary>
	public float movementSpeed = 1.0f;

	/// <summary>
	/// The m current way point.
	/// </summary>
	private int currentWayPoint = 0;

	/// <summary>
	/// The direction.
	/// </summary>
	private Vector3 direction;

	// Use this for initialization
	void Start () {

		direction = CalculateDirection (this.transform.position, waypointList [currentWayPoint].transform.position);
	}

	// Update is called once per frame
	void Update () {

		if (currentWayPoint < waypointList.Count) {


			float distance = Vector3.Distance (	this.transform.position, 
				               			 		waypointList [currentWayPoint].transform.position);

			if (distance > 10.0f) {

				Vector3 currentPosition = this.transform.position;

				float deltaSpeed = Time.deltaTime * movementSpeed;

				currentPosition.x += deltaSpeed * direction.x;
				currentPosition.y += deltaSpeed * direction.y;
				currentPosition.z += deltaSpeed * direction.z;

				this.transform.position = currentPosition;

			} else {

				if ((currentWayPoint + 1) < waypointList.Count) {
					currentWayPoint++;
					direction = CalculateDirection (this.transform.position, waypointList [currentWayPoint].transform.position);
				}
			}
		}
	}

	private Vector3 CalculateDirection(Vector3 current, Vector3 desitnation) {

		Vector3 destPosition = waypointList [currentWayPoint].transform.position;
		Vector3 currPosition = this.transform.position;

		return new Vector3 (	destPosition.x - currPosition.x,
								destPosition.y - currPosition.y,
								destPosition.z - currPosition.z).normalized;
	}
}
