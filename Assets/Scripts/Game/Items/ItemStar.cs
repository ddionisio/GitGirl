using UnityEngine;
using System.Collections;

//this only works when spawning
public class ItemStar : Entity, Entity.IListener {
	public enum LifeState {
		None,
		Active,
		Grabbed,
		Thrown,
		Dying
	}
			
	public tk2dAnimatedSprite starSprite;
	public tk2dBaseSprite glowSprite;
	
	public string starIdleClip;
	
	public float glowPulsePerSecond;
	public int glowPulseMinAlpha;
	
	public float disappearBlinkDelay;
	
	public float grabScale;
	
	public float throwSpeed;
	
	public int numBounce;
	
	public float throwFadeOffDelay; //decay after thrown
	public float fadeOffDelay; //when do we start disappearing?
	public float dyingDelay; //duration of disappear
		
	public bool resetMovementOnSpawn = true;
	
	private bool mFadeEnabled = false;
	
	private int mCurBounce = 0;
	
	private float mCurFadeTime = 0;
	private float mCurBlinkTime = 0;
	private float mCurPulseTime = 0;
	
	private float mGlowPulseMinAlpha;
	
	private LifeState mLifeState = LifeState.None;
	
	private bool mPlanetEnabledInitial = false;
	
	private bool mSceneActive = true;
	
	public bool fadeEnabled {
		get {
			return mFadeEnabled;
		}
		set {
			mFadeEnabled = value;
			mCurFadeTime = 0;
		}
	}
	
	protected override void Awake() {
		base.Awake();
		
		mPlanetEnabledInitial = planetAttach.enabled;
		
		mGlowPulseMinAlpha = ((float)glowPulseMinAlpha)/255.0f;
	}
	
	protected override void OnEnable() {
		base.OnEnable();
	}
		
	public void OnEntityAct(Action act) {
		switch(act) {
		case Action.spawning:
			fadeEnabled = false;
			
			starSprite.scale = Vector3.one;
			starSprite.color = Color.white;
			starSprite.Play(starIdleClip);
			
			glowSprite.scale = Vector3.one;
			glowSprite.color = Color.white;
			
			mCurBounce = 0;
			
			mLifeState = LifeState.None;
			
			planetAttach.enabled = mPlanetEnabledInitial;
			planetAttach.applyOrientation = true;
			planetAttach.applyGravity = false;
			gameObject.layer = Main.layerItem;
			mCollideLayerMask = 0;
			mReticle = Reticle.Type.Grab;
			
			if(resetMovementOnSpawn) {
				planetAttach.velocity = Vector2.zero;
				planetAttach.accel = Vector2.zero;
				planetAttach.ResetCurYVel();
			}
			
			mCurPulseTime = 0;
			break;
		}
	}
	
	public void OnEntitySpawnFinish() {
		mLifeState = LifeState.Active;
		action = Entity.Action.idle;
	}
	
	void OnGrabStart(PlayerGrabber grabber) {
		starSprite.color = Color.white;
		
		mLifeState = LifeState.Grabbed;
		planetAttach.enabled = false;
		fadeEnabled = false;
		mCurBounce = 0;
		InvulnerableOff();
	}
	
	void OnGrabDone(PlayerGrabber grabber) {
		Vector3 pos = transform.localPosition;
		pos.z = 0.0f;
		transform.localPosition = pos;
		
		starSprite.scale = new Vector3(grabScale, grabScale, starSprite.scale.z);
		
		glowSprite.scale = new Vector3(grabScale, grabScale, glowSprite.scale.z);
		
		grabber.Retract(true);
	}
	
	void OnGrabRetractStart(PlayerGrabber grabber) {
		//call proper state as 'grabbed'
	}
	
	void OnGrabRetractEnd(PlayerGrabber grabber) {
		//get eaten
	}
	
	void OnGrabDetach(PlayerGrabber grabber) {
		//put back in the world
		transform.parent = EntityManager.instance.transform;
		
		planetAttach.enabled = true;
		planetAttach.applyGravity = true;
	}
	
	void OnGrabThrow(PlayerGrabber grabber) {
		mLifeState = LifeState.Thrown;
		
		fadeEnabled = true;
		
		mCurBounce = 0;
		
		gameObject.layer = Main.layerPlayerProjectile;
		mCollideLayerMask = Main.layerMaskEnemyComplex;
		
		//compute velocity in planet space
		Vector2 dir = planetAttach.ConvertToPlanetDir(grabber.head.up);
		
		Vector2 throwVel = dir*throwSpeed;
		if(Mathf.Sign(dir.x) == Mathf.Sign(grabber.thePlayer.planetAttach.planetDir.x)) {
			throwVel += grabber.thePlayer.planetAttach.velocity;
		}
		
		if(grabber.thePlayer.planetAttach.GetCurYVel() > 0) {
			throwVel.y += grabber.thePlayer.planetAttach.GetCurYVel();
		}
		
		planetAttach.velocity = throwVel;
	}
	
	void OnPlanetLand(PlanetAttach pa) {
		Vector2 vel = planetAttach.velocity;
		vel.y = Mathf.Abs(vel.y);
		planetAttach.velocity = vel;
		planetAttach.ResetCurYVel();
		
		if(mCurBounce < numBounce) {
			mCurBounce++;
		}
	}
	
	public void OnEntityInvulnerable(bool yes) {
	}
	
	public void OnEntityCollide(Entity other, bool youAreReceiver) {
		//TODO: properly bounce off from their collision, really should pass in the ray hit data
		if((!youAreReceiver || mCollideLayerMask == Main.layerMaskEnemyComplex)
			&& mCurBounce < numBounce && mLifeState == LifeState.Thrown) {
			Vector2 vel = planetAttach.velocity;
			vel.x *= -1;
			planetAttach.velocity = vel;
			planetAttach.ResetCurYVel();
			
			mCurBounce++;
		}
	}
	
	void Die() {
		mCurFadeTime = 0;
		gameObject.layer = Main.layerIgnoreRaycast;
		mReticle = Reticle.Type.NumType;
		mLifeState = LifeState.Dying;
	}
	
	void Decay(float delay) {
		if(mFadeEnabled) {
			mCurFadeTime += Time.deltaTime;
			if(mCurFadeTime >= delay || mCurBounce >= numBounce) {
				mCurFadeTime = 0;
				gameObject.layer = Main.layerIgnoreRaycast;
				mReticle = Reticle.Type.NumType;
				mLifeState = LifeState.Dying;
			}
		}
	}
	
	void PulseGlow() {
		mCurPulseTime += Time.deltaTime;
		
		float t = Mathf.Sin(Mathf.PI*mCurPulseTime*glowPulsePerSecond);
		t *= t;
		
		Color c = glowSprite.color;
		c.a = mGlowPulseMinAlpha + t*(1.0f - mGlowPulseMinAlpha);
		glowSprite.color = c;
	}
	
	void LateUpdate () {
		switch(mLifeState) {
		case LifeState.Active:
			if(mSceneActive) {
				Decay(fadeOffDelay);
				PulseGlow();
			}
			else {
				Die();
			}
			break;
			
		case LifeState.Thrown:
			Decay(throwFadeOffDelay);
			PulseGlow();
			break;
			
		case LifeState.Grabbed:
			PulseGlow();
			break;
			
		case LifeState.Dying:
			mCurFadeTime += Time.deltaTime;
			if(mCurFadeTime >= dyingDelay) {
				mLifeState = LifeState.None;
				
				planetAttach.enabled = mPlanetEnabledInitial;
				EntityManager.instance.Release(transform);
			}
			
			mCurBlinkTime += Time.deltaTime;
			if(mCurBlinkTime >= disappearBlinkDelay) {
				mCurBlinkTime = 0.0f;
				
				Color c = starSprite.color;
				c.a = c.a == 1.0f ? 0.0f : 1.0f;
				
				starSprite.color = c;
				glowSprite.color = c;
			}
			break;
		}
	}
	
	void OnSceneActivate(bool yes) {
		mSceneActive = yes;
		
		if(!yes) {
			switch(mLifeState) {
			case LifeState.None:
			case LifeState.Active:
				Die();
				break;
			}
		}
	}
}
