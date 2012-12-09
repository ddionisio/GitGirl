using UnityEngine;
using System.Collections;

public class Tentacle : EntityBase {
	public int score;
	
	[System.NonSerialized]
	public BossLevelOne boss = null;
	
	void Awake() {
		mReticle = Reticle.Type.Eat;
	}
	
	void OnDestroy() {
		boss = null;
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
		grabber.player.AddScore(score);
		
		//make something happen
		grabber.DetachGrab();
		boss.TentacleEaten(this);
	}
}
