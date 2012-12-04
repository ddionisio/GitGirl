using UnityEngine;
using System.Collections;

public class ModalVictory : UIController {
	public UIEventListener buttonReturn;
	
	void OnButtonReturn(GameObject go) {
		SceneMainMenu.pushOnStart = UIManager.Modal.LevelSelect;
		
		Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
	}

	void Awake() {
		buttonReturn.onClick += OnButtonReturn;
	}
}
