using UnityEngine;
using System.Collections;

public class AISetAnimation : SequencerAction {
	
	public AIAnimKey[] animations;
	
	public float wait;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		ai.BroadcastMessage("OnAIAnimation", animations, SendMessageOptions.DontRequireReceiver);
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		//take into account time since animations were played so we are still in sync even after pause/resume of the sequencer
		return state.IsDelayReached(wait);
	}
}
