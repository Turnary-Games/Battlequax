using UnityEngine;
using System.Collections;

public class Artillery : _Weapon {
	
	public float degreesPerSecond = 10;
	public float degreesOffset = 0;

	void Update() {
		Vector3 delta = target - transform.position;
		Vector3 euler = transform.eulerAngles;

		euler.y = Mathf.MoveTowardsAngle (euler.y, Mathf.Atan2 (delta.x, delta.z) * Mathf.Rad2Deg, degreesPerSecond * Time.deltaTime);

		transform.eulerAngles = euler;
	}

}
