using UnityEngine;
using System.Collections;

public class HUDHitPoint : MonoBehaviour {
	public UISprite onSprite;
	
	public GameObject onWidget;
	public GameObject offWidget;
	
	public void SetOn(bool on) {
		if(on) {
			onSprite.color = Color.white;
		}
		
		onWidget.SetActiveRecursively(on);
		offWidget.SetActiveRecursively(!on);
	}
}
