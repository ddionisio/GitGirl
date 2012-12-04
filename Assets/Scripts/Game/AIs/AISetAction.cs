using UnityEngine;
using System.Collections;

public class AISetAction : SequencerAction {
	
	public Entity.Action act = Entity.Action.idle;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		ai.action = act;
	}
}
