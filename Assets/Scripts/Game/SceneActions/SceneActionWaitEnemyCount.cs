using UnityEngine;
using System.Collections;

public class SceneActionWaitEnemyCount : SequencerAction {
	public int amount = 0;

	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		return SceneLevel.instance.enemyCount <= amount;
	}
}
