using UnityEngine;
using System.Collections;

public class PlanetAttachRotateVelocity : MonoBehaviour {

	public PlanetAttach planetAttach;
	
	public float rotatePerMeter;
	
	private float mRotatePerMeterRad;
	
	void Awake() {
		mRotatePerMeterRad = rotatePerMeter*Mathf.Deg2Rad;
	}
	
	void Update() {
		if(planetAttach.velocity != Vector2.zero) {
			float vel = planetAttach.velocity.y == 0 ? planetAttach.velocity.x : planetAttach.velocity.magnitude;
			float rotate = mRotatePerMeterRad*vel*Time.deltaTime;
			
			Vector2 rotDir = Util.Vector2DRot(transform.up, rotate);
			
			Quaternion q = Quaternion.FromToRotation(transform.up, rotDir);
			transform.rotation = q*transform.rotation;
		}
	}
}
