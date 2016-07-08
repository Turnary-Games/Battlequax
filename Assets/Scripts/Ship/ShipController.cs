using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour {

	[Header("Camera")]
	public VirtualJoystick cameraJoystick;
	private Vector3 cameraEuler = new Vector3(10, 0, 0);
	public float cameraZoom = 10;
	public float cameraMinZoom = 3;
	public float cameraMaxZoom = 25;
	public Vector3 cameraOffset = Vector3.up * 3;
	public float cameraSpeed = 3;
	public float targetExtraDistance = 10f;

	[Header("Movement")]
	public VirtualWheel wheel;
	public VirtualThrottle throttle;

	private Rigidbody body;
	private List<_Weapon> weapons;
	private List<ShipEngine> engines;

	void Awake () {
		weapons = new List<_Weapon> (GetComponentsInChildren<_Weapon> ());
		engines = new List<ShipEngine> (GetComponentsInChildren<ShipEngine> ());
		body = GetComponent<Rigidbody> ();
	}

	void Update () {
		UpdateMovement ();
		UpdateWeapons ();
		UpdateCamera ();
	}

	void UpdateMovement() {
		float multiplier = throttle.value;
		float steering = wheel.valueNormalized;

		engines.ForEach (e => {
			Vector3 euler = e.transform.localEulerAngles;
			euler.y = Mathf.LerpAngle(e.minDegrees, e.maxDegrees, steering);
			e.transform.localEulerAngles = euler;

			float waveYPos = WaterController.current.GetWaveYPos(e.transform.position, Time.time);
			if (e.transform.position.y < waveYPos) {
				// Propellern måste vara i vattnet!
				Vector3 force = -e.transform.forward * e.power * multiplier;
				body.AddForceAtPosition(force, e.transform.position);
			}

		});

		Vector3 shipEuler = transform.eulerAngles;
		shipEuler.x = shipEuler.z = 0;
		transform.eulerAngles = shipEuler;
	}

	void UpdateCamera() {
		// Läs av input
		cameraEuler.y += cameraJoystick.input.x * cameraSpeed;
		cameraEuler.y %= 360;

		cameraEuler.x -= cameraJoystick.input.y * 0.5f * cameraSpeed;
		cameraEuler.x = Mathf.Clamp (cameraEuler.x, 1, 85);

		cameraZoom += Input.mouseScrollDelta.y;
		cameraZoom = Mathf.Clamp (cameraZoom, cameraMinZoom, cameraMaxZoom);

		// Change stuffs
		Camera.main.transform.eulerAngles = cameraEuler;
		Camera.main.transform.position = transform.TransformPoint(cameraOffset + Vector3.up * cameraZoom * .1f) - Camera.main.transform.forward * cameraZoom;
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
			water += Camera.main.transform.forward.y (0).normalized * targetExtraDistance;

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
		cameraMinZoom = Mathf.Max (cameraMinZoom, 0);
		cameraZoom = Mathf.Clamp (cameraZoom, cameraMinZoom, cameraMaxZoom);
	}
	#endif
}
