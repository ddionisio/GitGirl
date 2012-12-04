using UnityEngine;
using System.Collections;

public class AISetAccel : SequencerAction {
	public bool horizontalOnly = true;
	
	public float accel = 0;
	
	public float angle = 0;
	public bool useDir=false;
	public bool usePlayerDir=true;
				
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		PlanetAttach pa = ai.planetAttach;
		AIState aiState = (AIState)state;
		
		if(accel > 0) {
			if(usePlayerDir) {
				Player player = SceneLevel.instance.player;
				
				aiState.curPlanetDir = pa.GetDirTo(player.planetAttach, horizontalOnly);
			}
			else if(!useDir) {
				aiState.curPlanetDir = Util.Vector2DRot(new Vector2(1, 0), angle*Mathf.Deg2Rad);
			}
			
			pa.accel = aiState.curPlanetDir*accel;
		}
		else {
			pa.accel = Vector2.zero;
		}
	}
}