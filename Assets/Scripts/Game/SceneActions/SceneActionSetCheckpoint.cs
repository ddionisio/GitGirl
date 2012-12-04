using UnityEngine;
using System.Collections;

public class SceneActionSetCheckpoint : SequencerAction {
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aiState) {
		SceneLevel.instance.SetCheckpoint();
	}
}