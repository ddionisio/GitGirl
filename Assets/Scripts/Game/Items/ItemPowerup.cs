using UnityEngine;
using System.Collections;

public class ItemPowerup : Entity, Entity.IListener {

	protected override void Awake() {
		base.Awake();
		
		mReticle = Reticle.Type.Grab;
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
		Transform t = grabber.DetachGrab();
		
		Object.Destroy(t.gameObject);
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
