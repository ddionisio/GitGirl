using UnityEngine;
using System.Collections;

public class SceneActionOpenModal : SequencerAction {
	public UIManager.Modal modal;
	public bool waitForClose = false;

	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {	
		UIManager.instance.ModalOpen(modal);
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		return !(waitForClose && UIManager.instance.ModalIsInStack(modal));
	}
}
