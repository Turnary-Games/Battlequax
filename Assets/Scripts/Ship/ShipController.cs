using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

	public Vector3 cameraEuler = new Vector3(45, 90, 0);
	public float cameraDist = 10;
	public float cameraMinDist = 3;
	public float cameraMaxDist = 25;
	public Vector3 cameraOffset = Vector3.up * 3;

	private List<_Weapon> weapons;

	private Vector3 mousePos;
	private Vector3 mouseDelta;

	void Awake () {
		weapons = new List<_Weapon> (GetComponentsInChildren<_Weapon> ());
	}

	void Update () {
		CheckInput ();
		UpdateWeapons ();
		UpdateCamera ();
	}

	void CheckInput() {
		// OMG DAT LINE SO BEAUTIFUL
		var data = CustomStandaloneInputModule.GetLastPointerEventDataStatic(-1);
		if (data == null || data.pointerPress == null) {
			// Musen
			if (Input.GetMouseButtonDown (0))
				mousePos = Input.mousePosition;
		
			if (Input.GetMouseButton (0)) {
				mouseDelta = mousePos - Input.mousePosition;
				mousePos = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp (0))
				mouseDelta = Vector3.zero;

			// Kameran
			cameraEuler.y += mouseDelta.x;
			cameraEuler.y %= 360;

			cameraEuler.x += mouseDelta.y;
			cameraEuler.x = Mathf.Clamp (cameraEuler.x, 5, 85);

			cameraDist += Input.mouseScrollDelta.y;
			cameraDist = Mathf.Clamp (cameraDist, cameraMinDist, cameraMaxDist);
		}
	}

	void UpdateCamera() {
		Camera.main.transform.eulerAngles = cameraEuler;
		Camera.main.transform.position = transform.TransformPoint(cameraOffset + Vector3.up * cameraDist * .1f) - Camera.main.transform.forward * cameraDist;
	}

	void UpdateWeapons () {
		// Hitta x y av vattenytan kameran kollar på
		Vector3 pos = Camera.main.transform.position;
		float angle = cameraEuler.x;

		if (angle > 0 && angle < 180) {
			// cos A = b / c
			// c = b / cos A
			float dist = pos.y / Mathf.Sin (angle * Mathf.Deg2Rad);

			Vector3 water = pos + Camera.main.transform.forward * dist;

			// Sätt det som target för alla vapen
			weapons.ForEach (w => {
				w.target = water;
			});
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (Application.isPlaying && weapons != null) {
			weapons.Find (w => {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (Camera.main.transform.position, w.target);
				Gizmos.DrawSphere (w.target, .25f);

				return true;
			});

			weapons.ForEach (w => {
				Vector3 vec1 = w.target - w.transform.position;
				vec1.y = 0;
				vec1.Normalize ();
				vec1 *= 2.2f;

				Vector3 vec2 = w.transform.forward;
				vec2.y = 0;
				vec2.Normalize ();
				vec2 *= 2;

				Vector3 pos;

				Gizmos.color = Color.magenta;
				Gizmos.DrawRay (w.transform.position + Vector3.up * .1f, vec1);
				pos = w.transform.position + Vector3.up * .1f + vec1;
				Gizmos.DrawLine (pos, new Vector3 (pos.x, 0, pos.z));

				pos = w.transform.position + Vector3.up * .1f;
				Gizmos.DrawLine (pos, new Vector3 (pos.x, 0, pos.z));

				Gizmos.color = Color.green;
				Gizmos.DrawRay (w.transform.position, vec2);
				pos = w.transform.position + vec2;
				Gizmos.DrawLine (pos, new Vector3 (pos.x, 0, pos.z));

			});
		}
	}

	void OnValidate() {
		cameraMinDist = Mathf.Max (cameraMinDist, 0);
		cameraDist = Mathf.Clamp (cameraDist, cameraMinDist, cameraMaxDist);
	}
	#endif
}
