using UnityEngine;
using System.Collections;

public class ItemPowerup : Entity, Entity.IListener {

	protected override void Awake() {
		base.Awake();
		
		mReticle = Reticle.Type.Grab;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
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
		Transform t = grabber.DetachGrab();
		
		Object.Destroy(t.gameObject);
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
