using UnityEngine;
using System.Collections;

public class AIJump : SequencerAction {

	public float speedMin=0;
	public float speedMax=0;
	
	public Entity.Action actOnLand = Entity.Action.idle; //set to none or NumAction to keep jump state
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		PlanetAttach pa = ai.planetAttach;
		pa.Jump(speedMin < speedMax ? Random.Range(speedMin, speedMax) : speedMin);
		ai.action = Entity.Action.jump;
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		PlanetAttach pa = ai.planetAttach;
		
		bool done = pa.isGround;
		
		if(done && actOnLand != Entity.Action.none) {
			ai.action = actOnLand;
		}
		
		return pa.isGround;
	}
	
	public override void Finish(MonoBehaviour behaviour, Sequencer.StateInstance state) {
	}
}
