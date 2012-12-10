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
			
			gameObject.layer = Main.layerAutoGrab;
			//mReticle = Reticle.Type.Eat;
		}
		else {
			if(stateCallback != null) {
				stateCallback(this, State.Inactive);
			}
			
			gameObject.layer = Main.layerIgnoreRaycast;
			//mReticle = Reticle.Type.NumType;
		}
	}

	protected override void Awake() {
		base.Awake();
	}
		
	void OnGrabStart(PlayerGrabberBase grabber) {
		gameObject.layer = Main.layerIgnoreRaycast;
		
		if(stateCallback != null) {
			stateCallback(this, State.Grabbed);
		}
	}
	
	void OnGrabDone(PlayerGrabberBase grabber) {
		if(grabber.player.action == Entity.Action.die) {
		}
		else {
			grabber.player.stats.ApplyDamage(-1);
		}
		
		grabber.Retract(true);
	}
	
	void OnGrabRetractStart(PlayerGrabberBase grabber) {
	}
	
	void OnGrabRetractEnd(PlayerGrabberBase grabber) {
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
	
	public void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver) {
	}
	
	public void OnEntitySpawnFinish() {
	}
}
