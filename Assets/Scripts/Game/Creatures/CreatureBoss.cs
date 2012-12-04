using UnityEngine;
using System.Collections;

public class CreatureBoss : CreatureCommon {
	[System.Serializable]
	public class AIState {
		public int hp;
		public string state;
	}
	
	public AIState[] bossAIStates;
	
	public override void OnEntityAct(Action act) {
		base.OnEntityAct(act);
		
		switch(act) {
		case Action.spawning:
			UIManager.instance.hud.pointer.SetPOI(transform);
			UIManager.instance.hud.bossStatus.gameObject.SetActiveRecursively(true);
			UIManager.instance.hud.bossStatus.SetStats(stats);
			break;
			
		case Action.die:
			UIManager.instance.hud.pointer.SetPOI(null);
			break;
		}
	}
	
	public override string AIToStateAfterHurt() {
		string ret = null;
		
		if(stats != null) {
			foreach(AIState aiState in bossAIStates) {
				if(stats.curHP <= aiState.hp) {
					ret = aiState.state;
					break;
				}
			}
		}
		
		return ret;
	}
}
