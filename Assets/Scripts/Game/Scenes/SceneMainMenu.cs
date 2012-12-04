using UnityEngine;
using System.Collections;

public class SceneMainMenu : SceneController {
	public static UIManager.Modal pushOnStart = UIManager.Modal.NumModal;

	// Use this for initialization
	protected override void Start() {
		base.Start();
		
		UIManager.instance.ModalOpen(UIManager.Modal.Start);
		
		//put on top of start
		if(pushOnStart != UIManager.Modal.NumModal) {
			UIManager.instance.ModalOpen(pushOnStart);
			pushOnStart = UIManager.Modal.NumModal;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
