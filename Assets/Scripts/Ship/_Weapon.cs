using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class _Weapon : MonoBehaviour {

	public List<_Weapon> linkedWeapons = new List<_Weapon>();

	[System.NonSerialized]
	public Vector3 target;

	public float reloadTime = 3;
	private float lastFire = -Mathf.Infinity;
	public bool canShoot {
		get { return Time.time - lastFire >= reloadTime; }
	}
	public float reload_normalized {
		get { return Mathf.Clamp ((Time.time - lastFire) / reloadTime, 0, 1); }
	}
	public float reload_remaining {
		get { return Mathf.Clamp(reloadTime - Time.time + lastFire, 0, reloadTime); }
	}

	public abstract void OnFire ();
	public bool Fire() {
		if (canShoot) {
			lastFire = Time.time;
			OnFire ();

			foreach (_Weapon linked in linkedWeapons) {
				if (!linked) continue;
				if (linked == this) continue;
				linked.HiddenFire ();
			}

			return true;
		}
		return false;
	}

	private void HiddenFire() {
		lastFire = Time.time;
		OnFire ();
	}

}
