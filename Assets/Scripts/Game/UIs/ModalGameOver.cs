using UnityEngine;
using System.Collections;

public class ModalGameOver : UIController {
	public UIEventListener buttonRetry;
	public UIEventListener buttonReturn;
	
	void OnButtonRetry(GameObject go) {
		Main.instance.sceneManager.ReloadLevel();
	}
	
	void OnButtonReturn(GameObject go) {
		Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
	}

	void Awake() {
		buttonRetry.onClick += OnButtonRetry;
		buttonReturn.onClick += OnButtonReturn;
	}
}
