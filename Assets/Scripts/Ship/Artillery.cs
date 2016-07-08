using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Artillery : _Weapon {

	public GameObject projectilePrefab;
	public float projectileSpeed = 20;
	public float salvoDelay = .5f;
	public float barrelRecoil = .4f;
	public float barrelReturnXPos = 1.2f;
	public float barrelReturnSpeed = .3f;
	public List<Transform> fireAt = new List<Transform> ();
	public float degreesPerSecond = 10;
	public float degreesOffset = 0;

	void Update() {
		Vector3 delta = target - transform.position;
		Vector3 euler = transform.eulerAngles;

		euler.y = Mathf.MoveTowardsAngle (euler.y - degreesOffset, Mathf.Atan2 (delta.x, delta.z) * Mathf.Rad2Deg, degreesPerSecond * Time.deltaTime) + degreesOffset;

		transform.eulerAngles = euler;

		for (int i = 0; i < fireAt.Count; i++) {
			if (!fireAt [i]) continue;

			Transform parent = fireAt [i].parent;
			parent.localPosition = parent.localPosition.x (Mathf.MoveTowards (parent.localPosition.x, barrelReturnXPos, barrelReturnSpeed * Time.deltaTime));
		}
	}

	public override void OnFire () {
		StartCoroutine (FireSalvo ());
	}

	IEnumerator FireSalvo() {

		foreach (Transform t in fireAt) {
			if (!t) continue;

			Transform parent = t.parent;
			parent.localPosition = parent.localPosition.x (barrelReturnXPos - barrelRecoil);

			GameObject clone = Instantiate (projectilePrefab, t.position, t.rotation) as GameObject;

			Rigidbody body = clone.GetComponent<Rigidbody> ();
			body.AddForce (t.forward * projectileSpeed, ForceMode.Impulse);

			Destroy (clone, 15);

			yield return new WaitForSeconds (salvoDelay);
		}
	}

}
