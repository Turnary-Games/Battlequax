using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class FireButton : MonoBehaviour {

	public _Weapon weapon;
	public RectTransform loading;
	public Text text;
	private Button button;

	void Awake() {
		button = GetComponent<Button> ();
		button.onClick.AddListener (OnButtonClick);
	}

	void OnButtonClick() {
		weapon.Fire ();
	}

	void Update() {
		loading.anchorMax = loading.anchorMax.y (1-weapon.reload_normalized);
		button.interactable = weapon.canShoot;
		text.text = weapon.canShoot ? "FIRE" : (Mathf.Floor (weapon.reload_remaining * 10) * 0.1f).ToString();
	}
}
