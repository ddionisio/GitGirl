using UnityEngine;
using System.Collections;

public class ProjBullet : Entity, Entity.IListener {
	public float speed;
	
	[SerializeField] bool planetCollide = false;
	[SerializeField] bool isEnemy = true;
	[SerializeField] string aiState = null; //for more fancy stuff
	
	[SerializeField] float duration;
			
	[SerializeField] float deathDelay;
	
	private Vector2 mDir;
	
	private float mCurProjDelay;
	
	public Vector2 dir {
		get { return mDir; }
		set { mDir = value; }
	}
			
	public override bool CanHarmPlayer() {
		return isEnemy;
	}
		
	// Update is called once per frame
	void LateUpdate () {
		switch(action) {
		case Action.idle:
		case Action.move:
			if(planetAttach == null && string.IsNullOrEmpty(aiState)) {
				Vector3 deltaPos = (Time.deltaTime*speed)*mDir;
				transform.localPosition += deltaPos;
			}
			
			mCurProjDelay += Time.deltaTime;
			if(mCurProjDelay >= duration) {
				action = Action.die;
			}
			break;
			
		case Action.die:
			mCurProjDelay += Time.deltaTime;
			if(mCurProjDelay >= deathDelay) {
				Release();
			}
			break;
		}
	}
		
	//entity calls
	
	public void OnEntityAct(Action act) {
		mCurProjDelay = 0;
		
		switch(act) {
		case Action.spawning:
		case Action.die:
			gameObject.layer = Main.layerIgnoreRaycast;
			mCollideLayerMask = 0;
			break;
		}
	
	}
	
	public void OnEntityInvulnerable(bool yes) {
	
	}
	
	public void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver) {
		action = Action.die;
	}
	
	public void OnEntitySpawnFinish() {
		gameObject.layer = isEnemy ? Main.layerProjectile : Main.layerPlayerProjectile;
		
		if(!isEnemy) { //for complex enemies, bullet has to cast the ray for collisions
			mCollideLayerMask = Main.layerMaskEnemyComplex;
		}
		
		if(planetCollide) {
			mCollideLayerMask |= Main.layerMaskPlanet;
		}
		
		//projectiles are always on the move
		action = Action.move;
		
		if(!string.IsNullOrEmpty(aiState)) {
			AISetState(aiState);
		}
		else {
			if(planetAttach != null && speed > 0) {
				planetAttach.velocity = planetAttach.ConvertToPlanetDir(mDir)*speed;
			}
		}
	}
}
