using UnityEngine;
using System.Collections;

public class ItemWeapon : Entity, Entity.IListener {
	public Weapon.Type weapon;

	protected override void Awake() {
		base.Awake();
		
		ReadyForGrab();
	}
	
	void OnGrabStart(PlayerGrabberBase grabber) {
		gameObject.layer = Main.layerIgnoreRaycast;
	}
	
	void OnGrabDone(PlayerGrabberBase grabber) {
		grabber.Retract(true);
	}
	
	void OnGrabRetractStart(PlayerGrabberBase grabber) {
	}
	
	void OnGrabRetractEnd(PlayerGrabberBase grabber) {
		//make something happen
		grabber.DetachGrab();
		
		Release();
		
		PlayerGrabber pGrabber = grabber as PlayerGrabber;
		if(pGrabber != null) {
			pGrabber.Equip(weapon);
		}
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
		ReadyForGrab();
	}
	
	void ReadyForGrab() {
		gameObject.layer = Main.layerItem;
		mReticle = Reticle.Type.Grab;
	}
}