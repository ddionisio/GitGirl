using UnityEngine;
using System.Collections;

public class ModalStart : UIController {
	public UIEventListener buttonStart;
	public UIEventListener buttonHowTo;
	
	void Awake() {
		buttonStart.onClick += OnButtonStart;
		
		if(buttonHowTo != null)
			buttonHowTo.onClick += OnButtonHowTo;
	}
	
	void OnButtonStart(GameObject go) {
		UIManager.instance.ModalOpen(UIManager.Modal.LevelSelect);
	}
	
	void OnButtonHowTo(GameObject go) {
		UIManager.instance.ModalOpen(UIManager.Modal.HowToPlay);
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
