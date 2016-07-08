using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class VirtualWheel : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

	public float degreesOffset;
	public float minAngle = 90;
	public float maxAngle = 270;
	public float steps = 1;
	private RectTransform rect;

	private float startAngle;
	private float deltaAngle;
	public float angle;

	public float value {
		get { return Mathf.Clamp (angle, minAngle, maxAngle) - degreesOffset - minAngle; }
	}
	public float valueUnclamped {
		get { return angle - degreesOffset - minAngle; }
	}
	public float valueNormalized {
		get { return Mathf.InverseLerp (minAngle, maxAngle, angle); }
	}

	void Awake() {
		rect = GetComponent<RectTransform> ();
		angle = rect.localEulerAngles.z;
	}

	public void OnBeginDrag(PointerEventData e) {
		startAngle = angle;
		deltaAngle = 0;
		e.Use ();
	}

	public void OnEndDrag(PointerEventData e) {
		angle = Mathf.Clamp (angle, minAngle, maxAngle);
		e.Use ();
	}

	public void OnDrag(PointerEventData e) {
		Vector2 delta = e.position - rect.TransformPoint (Vector3.zero).xy ();
		Vector3 euler = rect.localEulerAngles;

		deltaAngle += Mathf.DeltaAngle (angle, Round(delta.ToDegrees () + degreesOffset));
		angle = startAngle + deltaAngle;


		euler.z = Mathf.Clamp (angle, minAngle, maxAngle);
		rect.localEulerAngles = euler;

		e.Use ();
	}

	float Round(float angle) {
		if (steps > 0)
			angle = Mathf.Round (angle / steps) * steps;
		return angle;
	}
}
