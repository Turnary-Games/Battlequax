using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VirtualThrottle : MonoBehaviour {

	public int roundWhenBelow = 1;
	private Slider slider;
	public float value {
		get { return slider.value < roundWhenBelow ? Mathf.Round (slider.value) : slider.value; }
	}

	void Awake () {
		slider = GetComponent<Slider> ();
	}

	void LateUpdate () {
		if (slider.value < roundWhenBelow)
			slider.value = Mathf.Round (slider.value);
	}
}
