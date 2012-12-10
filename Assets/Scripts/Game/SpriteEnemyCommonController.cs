using UnityEngine;
using System.Collections;

public class SpriteEnemyCommonController : SpriteEntityController {
	[SerializeField] CreatureCommon creature; //the creature this is attached to
	[SerializeField] bool flipOnStun = false;
	
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start() {
		base.Start();
	}
	
	protected override void Update() {
		base.Update();
		
		//set to fall state if applicable
		if(creature != null && creature.planetAttach.applyGravity && creature.action != Entity.Action.stunned) {
			if(creature.planetAttach.GetCurYVel() < 0) {
				PlayAnim(Entity.Action.fall);	
			}
		}
	}

	public override void OnEntityAct(Entity.Action act) {
		base.OnEntityAct(act);
		
		switch(act) {
		case Entity.Action.spawning:
		case Entity.Action.idle:
			ResetCommonData();
			break;
			
		case Entity.Action.stunned:
			if(flipOnStun) {
				Vector2 scale;
				scale = mSprite.scale;
				scale.y = -Mathf.Abs(scale.y);
				mSprite.scale = scale;
			}
			break;
		}
	}
	
	void OnPlanetLand(PlanetAttach pa) {
		//perform proper animation
		if(creature != null) {
			if(creature.action != Entity.Action.NumActions)
				PlayAnim(creature.action);
		}
	}
	
	void ResetCommonData() {
		if(flipOnStun) {
			Vector2 scale;
			scale = mSprite.scale;
			scale.y = Mathf.Abs(scale.y);
			mSprite.scale = scale;
		}
	}
}
