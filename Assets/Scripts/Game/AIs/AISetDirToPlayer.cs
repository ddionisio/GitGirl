using UnityEngine;
using System.Collections;

public class AISetDirToPlayer : SequencerAction {
	public bool horizontalOnly = true;
			
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		PlanetAttach pa = ai.planetAttach;
		Player player = SceneLevel.instance.player;
		
		((AIState)state).curPlanetDir = pa.GetDirTo(player.planetAttach, horizontalOnly);
	}
}
