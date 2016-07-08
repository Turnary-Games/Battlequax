using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

	public float maxDist = 100;
	public float deadzone = 0.01f;
	[System.NonSerialized]
	public Vector2 input;
	private RectTransform rect;

	void Awake() {
		rect = GetComponent<RectTransform> ();
	}

	public void OnBeginDrag(PointerEventData e) {
		e.Use ();
	}

	public void OnEndDrag(PointerEventData e) {
		rect.anchoredPosition = input = Vector2.zero;

		e.Use ();
	}

	public void OnDrag(PointerEventData e) {
		rect.anchoredPosition = Vector2.ClampMagnitude (rect.InverseTransformPoint(e.position).xy() + rect.anchoredPosition, maxDist);

		input = (rect.anchoredPosition / maxDist);
		// Deadzone i mitten
		if (input.magnitude <= deadzone) input = Vector2.zero;
		// Gör input'n exponenciell
		input *= input.sqrMagnitude;

		e.Use ();
	}
}
