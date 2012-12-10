using UnityEngine;
using System.Collections;

public class SirRobert : Entity, Entity.IListener {
	public Transform heartHolder;
	
	public tk2dBaseSprite heartGlow;
	
	public float heartRegenDelay = 3.0f;
	
	public int healthCriteria = 3;
	
	public float minPlayerDistance = 60; //based on planet space
	public float minPlayerNeedHeartDistance = 60; //based on planet space
	
	public float acceleration;
	
	public float heartGlowPulsePerSecond;
	
	private ItemHeart mHeart;
	private float mCurTime = 0;
	private float mCurHeartGlowTime = 0;
	private ItemHeart.State mCurHeartState;
	private Player mPlayer = null;
	private float mMinDist;
	
	protected override void Awake() {
		base.Awake();
		
		mHeart = heartHolder.GetComponentInChildren<ItemHeart>();
		mHeart.stateCallback = OnHeartStateChange;
		
		mMinDist = minPlayerDistance;
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		mHeart.Activate(false);
	}
	
	bool PlayerNeedHeal() {
		return mPlayer.stats.curHP < healthCriteria;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch(action) {
		case Entity.Action.idle:
		case Entity.Action.move:
			if(mPlayer == null) {
				SceneLevel sl = SceneLevel.instance;
				if(sl != null) {
					mPlayer = sl.player;
				}
			}
			else {
				bool needHeal = PlayerNeedHeal();
				
				//follow player
				if(planetAttach.GetDistanceHorizontal(mPlayer.planetAttach) > mMinDist) {
					Vector2 dir = planetAttach.GetDirTo(mPlayer.planetAttach, true);
					planetAttach.accel = dir*acceleration;
					action = Entity.Action.move;
				} 
				else if(planetAttach.velocity.x != 0) {
					planetAttach.velocity = Vector2.zero;
					planetAttach.accel = Vector2.zero;
					action = Entity.Action.idle;
				}
				
				//update heart
				switch(mCurHeartState) {
				case ItemHeart.State.Inactive:
					if(needHeal) {
						mCurTime += Time.deltaTime;
						if(mCurTime >= heartRegenDelay) {
							mHeart.Activate(true);
						}
						
						mMinDist = minPlayerNeedHeartDistance;
					}
					else {
						mMinDist = minPlayerDistance;
					}
					break;
				}
				
				if(needHeal) {
					if(!heartGlow.gameObject.active) {
						heartGlow.gameObject.active = true;
					}
					
					mCurHeartGlowTime += Time.deltaTime;
					float t = Mathf.Sin(Mathf.PI*mCurHeartGlowTime*heartGlowPulsePerSecond);
					t *= t;
					Color c = heartGlow.color; c.a = t;
					heartGlow.color = c;
				}
				else if(heartGlow.gameObject.active) {
					heartGlow.gameObject.active = false;
				}
			}
			break;
		}
	}
	
	protected override void OnDestroy() {
		base.OnDestroy();
		
		mHeart = null;
		mPlayer = null;
	}
	
	public void OnEntityAct(Action act) {
		switch(act) {
		case Action.spawning:
			mHeart.Activate(false);
			break;
		}
	}
	
	public void OnEntityInvulnerable(bool yes) {
	}
	
	public void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver) {
	}
	
	public void OnEntitySpawnFinish() {
		action = Entity.Action.idle;
	}
			
	void OnHeartStateChange(ItemHeart heart, ItemHeart.State state) {
		mCurTime = 0;
		
		mCurHeartState = state;
		switch(mCurHeartState) {
		case ItemHeart.State.Inactive:
			heartHolder.gameObject.SetActiveRecursively(false);
			break;
		case ItemHeart.State.Active:
			heartHolder.gameObject.SetActiveRecursively(true);
			break;
		case ItemHeart.State.Grabbed:
			break;
		case ItemHeart.State.Eaten:
			heart.transform.parent = heartHolder;
			heart.transform.localPosition = Vector3.zero;
			heart.transform.localRotation = Quaternion.identity;
			heart.transform.localScale = Vector3.one;
			heart.Activate(false); //will set state to inactive
			break;
		}
	}
	
	void OnSceneCounterCheck(Sequencer.StateInstance state) {
		state.counter = PlayerNeedHeal() ? 0 : 1;
	}
}
