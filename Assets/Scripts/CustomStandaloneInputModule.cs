using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomStandaloneInputModule : StandaloneInputModule {

	public PointerEventData GetLastPointerEventDataPublic (int id) {
		return GetLastPointerEventData (id);
	}

	public static PointerEventData GetLastPointerEventDataStatic (int id) {
		return EventSystem.current.GetComponent<CustomStandaloneInputModule> ().GetLastPointerEventDataPublic (id);
	}

}
