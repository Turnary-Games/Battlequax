using UnityEngine;
using System.Collections;

public class ShipEngine : MonoBehaviour {

	public float power = 10;
	public float minDegrees = 0;
	public float maxDegrees = 180;

	void OnDrawGizmos() {
		Vector3 force = transform.forward;
		Gizmos.color = Color.red;
		Gizmos.DrawRay (transform.position, force);
	}
}
