using UnityEngine;
using System.Collections;

public class ItemWeapon : Entity, Entity.IListener {
	public Weapon.Type weapon;

	protected override void Awake() {
		base.Awake();
		
		mReticle = Reticle.Type.Grab;
	}
	
	void OnGrabStart(PlayerGrabber grabber) {
	}
	
	void OnGrabDone(PlayerGrabber grabber) {
		grabber.Retract(true);
	}
	
	void OnGrabRetractStart(PlayerGrabber grabber) {
	}
	
	void OnGrabRetractEnd(PlayerGrabber grabber) {
		//make something happen
		grabber.DetachGrab();
		
		Release();
		
		grabber.Equip(weapon);
	}
	
	public void OnEntityAct(Action act) {
		switch(act) {
		case Action.spawning:
			break;
		}
	}
	
	public void OnEntityInvulnerable(bool yes) {
	}
	
	public void OnEntityCollide(Entity other, bool youAreReceiver) {
	}
	
	public void OnEntitySpawnFinish() {
		mReticle = Reticle.Type.Grab;
	}
}