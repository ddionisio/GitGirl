using UnityEngine;
using System.Collections;

public class ItemHeart : Entity, Entity.IListener {
	public enum State {
		Inactive,
		Active,
		Grabbed,
		Eaten
	}
	
	public delegate void OnStateChange(ItemHeart heart, State state);
	
	public OnStateChange stateCallback;
	
	public void Activate(bool active) {
		if(active) {
			if(stateCallback != null) {
				stateCallback(this, State.Active);
			}
			
			gameObject.layer = Main.layerItem;
			mReticle = Reticle.Type.Eat;
		}
		else {
			if(stateCallback != null) {
				stateCallback(this, State.Inactive);
			}
			
			gameObject.layer = Main.layerIgnoreRaycast;
			mReticle = Reticle.Type.NumType;
		}
	}

	protected override void Awake() {
		base.Awake();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	void OnGrabStart(PlayerGrabber grabber) {
		if(stateCallback != null) {
			stateCallback(this, State.Grabbed);
		}
	}
	
	void OnGrabDone(PlayerGrabber grabber) {
		if(grabber.thePlayer.action == Entity.Action.die) {
		}
		else {
			grabber.thePlayer.stats.ApplyDamage(-1);
		}
		
		grabber.Retract(true);
	}
	
	void OnGrabRetractStart(PlayerGrabber grabber) {
	}
	
	void OnGrabRetractEnd(PlayerGrabber grabber) {
		//make something happen
		grabber.DetachGrab();
		
		if(stateCallback != null) {
			stateCallback(this, State.Eaten);
		}
	}
	
	protected override void OnDestroy() {
		base.OnDestroy();
		
		stateCallback = null;
	}
	
	public void OnEntityAct(Action act) {
	}
	
	public void OnEntityInvulnerable(bool yes) {
	}
	
	public void OnEntityCollide(Entity other, bool youAreReceiver) {
	}
	
	public void OnEntitySpawnFinish() {
	}
}
