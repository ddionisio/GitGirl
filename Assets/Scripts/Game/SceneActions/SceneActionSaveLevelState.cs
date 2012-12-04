using UnityEngine;
using System.Collections;

public class SceneActionSaveLevelState : SequencerAction {
	public UserData.LevelState state = UserData.LevelState.Complete;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance _state) {
		Main.instance.userData.SetCurrentLevelState(state);
		Main.instance.userData.SetCurrentLevelScore(SceneLevel.instance.player.GetScore());
	}
}
