using UnityEngine;
using System.Collections;

public class PlayerSpriteController : SpriteEntityController {
	[SerializeField] Player player; //the player this is attached to
	
	void OnDestroy() {
		player = null; //just in case
	}
	
	// Update is called once per frame
	protected override void Update() {
		base.Update();
		
		if(!(player.action == Entity.Action.hurt || player.action == Entity.Action.start)) {
			if(player.planetAttach.GetCurYVel() > 0) {
				PlayAnim(Entity.Action.jump);
			}
			else if(player.planetAttach.GetCurYVel() < 0) {
				if(player.action == Entity.Action.jump)
					PlayAnim(Entity.Action.jump_fall);
				else
					PlayAnim(Entity.Action.fall);
			}
		}
	}
	
	public override void OnEntityAct(Entity.Action act) {
		bool doPlay = true;
		
		//don't play until player lands
		if(!(act == Entity.Action.hurt || act == Entity.Action.jump || act == Entity.Action.start)) {
			doPlay = player.planetAttach.isGround;
		}
		
		if(doPlay) {
			PlayAnim(act);
		}
	}
	
	void OnPlanetLand(PlanetAttach pa) {
		//perform proper animation
		switch(player.action) {
		case Entity.Action.hurt:
			break;
		default:
			PlayAnim(player.action);
			break;
		}
	}
}
