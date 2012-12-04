using UnityEngine;
using System.Collections;

public class AIFollowPlayerAccel : SequencerAction {
	public bool horizontalOnly = true;
	
	public float accel = 0;
	
	public int doneAfterNumChangeDir = 1;
	
	public float breakSpeed = 0; //cheap
	public bool useBreakSpeed = true;
			
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		((AIState)state).counter = 0;
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		AIState aiState = (AIState)state;
		Entity ai = (Entity)behaviour;
		
		PlanetAttach pa = ai.planetAttach;
		Player player = SceneLevel.instance.player;
		
		//Debug.Log("side: "+pa.CheckSide(player.planetAttach));
		Vector2 prevDir = aiState.curPlanetDir;
		aiState.curPlanetDir = pa.GetDirTo(player.planetAttach, horizontalOnly);
		
		bool done = true;
		
		//cap speed on opposite dir
		if(Vector2.Dot(prevDir, aiState.curPlanetDir) < 0.0f) {			
			if(useBreakSpeed) {
				pa.velocity = prevDir*breakSpeed;
			}
			
			if(doneAfterNumChangeDir > 0 && aiState.counter < doneAfterNumChangeDir) {
				aiState.counter++;
			}
		}
		
		if(doneAfterNumChangeDir > 0) {
			done = doneAfterNumChangeDir == aiState.counter && Vector2.Dot(pa.planetDir, aiState.curPlanetDir) > 0.0f;
		}
		
		//pa.velocity = Vector2.zero;
		pa.accel = aiState.curPlanetDir*accel;
				
		return done;
	}
}
