using UnityEngine;
using System.Collections;

public class AISetVelocity : SequencerAction {
	public float speedMin = 0;
	public float speedMax = 0;
	public float angle = 0; //0 to 360, counterclockwise from right
	
	public bool followPlayer = false;
	public bool followPlayerHorizontal = true;
	public float followPlayerDuration = 0;
	
	public bool useDir = false;
	public bool resetAccel = true;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		PlanetAttach pa = ai.planetAttach;
		AIState aiState = (AIState)state;
		
		float speed = speedMin < speedMax ? Random.Range(speedMin, speedMax) : speedMin;
		if(speed > 0) {
			if(followPlayer) {
				Player player = SceneLevel.instance.player;
				
				aiState.curPlanetDir = pa.GetDirTo(player.planetAttach, followPlayerHorizontal);
			}
			else if(!useDir) {
				aiState.curPlanetDir = Util.Vector2DRot(new Vector2(1, 0), angle*Mathf.Deg2Rad);
			}
			
			pa.velocity = aiState.curPlanetDir*speed;
			
			ai.action = Entity.Action.move;
		}
		else {
			pa.velocity = Vector2.zero;
			
			ai.action = Entity.Action.idle;
		}
		
		if(resetAccel) {
			pa.accel = Vector2.zero;
		}
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		bool done = true;
		
		if(followPlayer && !state.IsDelayReached(followPlayerDuration)) {
			Entity ai = (Entity)behaviour;
			PlanetAttach pa = ai.planetAttach;
			AIState aiState = (AIState)state;
			Player player = SceneLevel.instance.player;
			
			aiState.curPlanetDir = pa.GetDirTo(player.planetAttach, followPlayerHorizontal);
			
			if(followPlayerHorizontal) {
				pa.velocity = new Vector2(Mathf.Abs(pa.velocity.x)*aiState.curPlanetDir.x, pa.velocity.y);
			}
			else {
				pa.velocity = aiState.curPlanetDir*pa.velocity.magnitude;
			}
			
			done = false;
		}
		
		return done;
	}
}
