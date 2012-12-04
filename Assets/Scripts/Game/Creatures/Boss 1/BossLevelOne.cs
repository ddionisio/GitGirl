using UnityEngine;
using System.Collections;

public class BossLevelOne : CreatureBoss {
	public Transform body;
	
	public float tentacleHurtDelay = 2;
	public float tentacleEyesExposedDelay = 5;
	public float tentacleEyesExposedInvulDelay = 1;
	public float tentacleRegenDelay = 2;
	
	public float bodyShakeDelay = 0.1f;
	public float bodyShakeLen = 10.0f;
	
	private enum Status {
		Active,
		TentacleEaten,
		EyesExposed,
		TentacleRegen, //regenerating
	}
	
	private Tentacle[] mTentacles;
	private Transform[] mTentacleParents;
	private int mNumActiveTentacles;
	
	private Eye[] mEyes;
	private Transform[] mEyeParents;
	private int mNumActiveEyes;
	
	private float mCurTime = 0;
	private float mCurShakeTime = 0;
	
	private Status mStatus = Status.Active;
	
	private Vector3 mPrevBodyPos;
	
	private bool mFirstLand;
		
	//only used by eye
	public void EyeEaten(Eye e) {
		bool eaten = false;
		
		for(int i = 0; i < mEyes.Length; i++) {
			if(mEyes[i] == e) {
				e.transform.parent = mEyeParents[i];
				e.transform.localPosition = Vector3.zero;
				e.transform.localRotation = Quaternion.identity;
				e.transform.localScale = Vector3.one;
				mEyeParents[i].gameObject.SetActiveRecursively(false);
				mNumActiveEyes--;
				eaten = true;
				break;
			}
		}
		
		if(eaten) {
			stats.ApplyDamage(1);
			DoHurt();
		}
	}
	
	//only used by tentacle
	public void TentacleEaten(Tentacle t) {
		bool eaten = false;
		
		for(int i = 0; i < mTentacles.Length; i++) {
			if(mTentacles[i] == t) {
				t.transform.parent = mTentacleParents[i];
				t.transform.localPosition = Vector3.zero;
				t.transform.localRotation = Quaternion.identity;
				t.transform.localScale = Vector3.one;
				mTentacleParents[i].gameObject.SetActiveRecursively(false);
				mNumActiveTentacles--;
				eaten = true;
				break;
			}
		}
		
		if(eaten) {
			//reset tentacle targets, make them untargettable, get hurt a bit
			if(mNumActiveTentacles == 0) {
				//eyes are vulnerable, sit helpless in tears
				SetStatus(Status.EyesExposed);
			}
			else { //ouch
				SetStatus(Status.TentacleEaten);
			}
		}
	}
	
	protected override void OnDestroy() {
		base.OnDestroy();
		
		mTentacleParents = null;
		mTentacles = null;
		
		mEyes = null;
		mEyeParents = null;
	}
	
	public override bool CanHarmPlayer() {
		return true;
	}
			
	private void Setup() {
		mFirstLand = false; //once landed, enable tentacles
		
		gameObject.layer = Main.layerEnemyNoPlayerProjectile;
		
		mStatus = Status.Active;
		
		//tentacles
		if(mTentacles == null) {
			mTentacles = GetComponentsInChildren<Tentacle>(true);
			mTentacleParents = new Transform[mTentacles.Length];
		}
		
		for(int i = 0; i < mTentacles.Length; i++) {
			Tentacle t = mTentacles[i];
			//t.gameObject.SetActiveRecursively(true);
			t.gameObject.layer = Main.layerEnemyNoPlayerProjectile;
			t.boss = this;
			mTentacleParents[i] = t.transform.parent;
			
		}
		
		mNumActiveTentacles = mTentacles.Length;
		
		//eyes
		if(mEyes == null) {
			mEyes = GetComponentsInChildren<Eye>(true);
			mEyeParents = new Transform[mEyes.Length];
		}
		
		for(int i = 0; i < mEyes.Length; i++) {
			Eye e = mEyes[i];
			//e.gameObject.SetActiveRecursively(true);
			e.gameObject.layer = Main.layerEnemyNoPlayerProjectile;
			e.boss = this;
			mEyeParents[i] = e.transform.parent;
			
		}
		
		mNumActiveEyes = mEyes.Length;
	}
	
	private void SetStatus(Status s) {
		mStatus = s;
		switch(s) {
		case Status.Active:
			AIRestart();
			break;
			
		case Status.TentacleEaten:
			SetTentacleVulnerable(false);
			
			Invulnerable(tentacleHurtDelay);
			break;
			
		case Status.EyesExposed:
			StopMotion();
			
			SetEyesVulnerable(true);
			
			Invulnerable(tentacleEyesExposedInvulDelay); //just for the blinking
			
			mCurTime = 0;
			break;
			
		case Status.TentacleRegen:
			StopMotion();
			
			SetEyesVulnerable(false);
			
			mPrevBodyPos = body.localPosition;
			mCurShakeTime = mCurTime = 0;
			break;
		}
	}
	
	private void StopMotion() {
		planetAttach.velocity = Vector2.zero;
		planetAttach.accel = Vector2.zero;
		AISetPause(true);
	}
	
	private void RegenTentacles() {
		//TODO: animation of sort
		for(int i = 0; i < mTentacles.Length; i++) {
			Tentacle t = mTentacles[i];
			t.gameObject.layer = Main.layerItem;
			mTentacleParents[i].gameObject.SetActiveRecursively(true);
		}
		mNumActiveTentacles = mTentacles.Length;
	}
			
	private void SetTentacleVulnerable(bool yes) {
		for(int i = 0; i < mTentacles.Length; i++) {
			if(mTentacleParents[i].gameObject.active) {
				Tentacle t = mTentacles[i];
				t.gameObject.layer = yes ? Main.layerItem : Main.layerEnemyNoPlayerProjectile;
			}
		}
		
		if(!yes) {
			Main.instance.reticleManager.DeactivateAll();
		}
	}
	
	private void SetEyesVulnerable(bool yes) {
		for(int i = 0; i < mEyes.Length; i++) {
			if(mEyeParents[i].gameObject.active) {
				Eye e = mEyes[i];
				e.gameObject.layer = yes ? Main.layerItem : Main.layerEnemyNoPlayerProjectile;
			}
		}
		
		if(!yes) {
			Main.instance.reticleManager.DeactivateAll();
		}
	}
	
	public override void OnEntityInvulnerable(bool yes) {
		base.OnEntityInvulnerable(yes);
		
		//tentacle was eaten, get back to moving
		if(mStatus == Status.TentacleEaten && !yes) {
			SetTentacleVulnerable(true);
			mStatus = Status.Active;
		}
	}
	
	/*if(!yes && action == Entity.Action.hurt) {*/
	
	public override void OnEntityAct(Action act) {
		base.OnEntityAct(act);
		
		switch(act) {
		case Action.spawning:
			Setup();
			break;
			
		case Action.hurt:
			SetEyesVulnerable(false); //exposed later
			break;
			
		case Action.die:
			break;
			
		default:
			//finished getting hurt
			if(prevAction == Entity.Action.hurt) {
				SetStatus(Status.TentacleRegen);
			}
			break;
		}
	}
	
	public override void OnEntitySpawnFinish() {
		action = Entity.Action.idle;
		
		base.OnEntitySpawnFinish();
	}
	
	protected override void LateUpdate () {
		base.LateUpdate();
		
		switch(action) {
		case Action.hurt:
			break;
			
		case Action.die:
			break;
			
		default:
			switch(mStatus) {
			case Status.EyesExposed:
				mCurTime += Time.deltaTime;
				if(mCurTime >= tentacleEyesExposedDelay) {
					SetStatus(Status.TentacleRegen);
				}
				break;
				
			case Status.TentacleRegen:
				mCurShakeTime += Time.deltaTime;
				if(mCurShakeTime >= bodyShakeDelay) {
					mCurShakeTime = 0;
					Vector2 blah = Random.insideUnitCircle*bodyShakeLen;
					Vector3 shakePos = mPrevBodyPos;
					shakePos.x = blah.x; shakePos.y = blah.y;
					body.transform.localPosition = shakePos;
				}
				
				mCurTime += Time.deltaTime;
				if(mCurTime >= tentacleRegenDelay) {
					body.transform.localPosition = mPrevBodyPos;
					
					RegenTentacles();
					SetStatus(Status.Active);
				}
				break;
			}
			break;
		}
	}
	
	void OnPlanetLand(PlanetAttach pa) {
		if(!mFirstLand) {
			mFirstLand = true;
			SetTentacleVulnerable(true);
		}
	}
}
