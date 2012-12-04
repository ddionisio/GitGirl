using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
	public string exitConfirmTitle;
	public string exitConfirmMessage;
	
	public UIEventListener buttonResume;
	public UIEventListener buttonReturn;
		
	void OnButtonResume(GameObject go) {
		Main.instance.sceneManager.Resume();
		
		UIManager.instance.ModalCloseAll();
	}
	
	void OnButtonReturn(GameObject go) {
		ModalConfirm.Open(exitConfirmTitle, exitConfirmMessage, OnConfirm);
	}

	void Awake() {
		buttonResume.onClick += OnButtonResume;
		buttonReturn.onClick += OnButtonReturn;
	}
	
	void OnConfirm(bool yes) {
		if(yes) {
			Main.instance.sceneManager.Resume();
			Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
		}
	}
}
