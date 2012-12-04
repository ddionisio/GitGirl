using UnityEngine;
using System.Collections;

public class PlanetAttach : PlanetAttachStatic {
	public const float maxYVelCap = 600.0f;
	
	public bool applyGravity = true;
	public bool applyOrientation = true;
	
	public float maxVelocity;
	
	[System.NonSerialized]
	public Vector2 velocity;
	
	[System.NonSerialized]
	public Vector2 accel;
	
	private int mJumpCounter = 0;
	private bool mIsGround = false;
	
	private float mYVel = 0;
	private float mMaxVelocitySq;
	
	public int jumpCounter {
		get {
			return mJumpCounter;
		}
	}
	
	public bool isGround {
		get {
			return mIsGround;
		}
	}
	
	//this is the jump and gravity velocity combined along y
	public float GetCurYVel() {
		return mYVel;
	}
	
	//fake physics
	public void Jump(float vel, bool incJumpCounter=true) {
		mIsGround = false;
		mYVel = vel;
		
		if(incJumpCounter)
			mJumpCounter++;
	}
	
	public void ResetCurYVel() {
		mYVel = 0.0f;
	}
	
	protected override void OnAdjustToGround() {
		mYVel = 0;
		mJumpCounter = 0;
		
		if(!mIsGround) {
			mIsGround = true;
			gameObject.BroadcastMessage("OnPlanetLand", this, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	protected override void OnEnable() {
		base.OnEnable();
		
		mIsGround = false;
		mYVel = 0;
	}
	
	protected override void Awake () {
		base.Awake ();
		
		mMaxVelocitySq = maxVelocity*maxVelocity;
	}
	
	void LateUpdate() {
		float dt = Time.deltaTime;
		
		Vector3 _planetPos = planet.transform.position;
						
		//orient to planet
		if(applyOrientation) {
			Vector2 mDirToPlanet = _planetPos - mTrans.position;
			mDirToPlanet.Normalize();
			
			Quaternion q = Quaternion.FromToRotation(mTrans.up, -mDirToPlanet);
			mTrans.rotation = q * mTrans.rotation;
		}
		
		if(!mIsGround && applyGravity) {
			mYVel += planet.gravity*dt;
			
			if(mYVel > maxYVelCap) {
				mYVel = maxYVelCap;
			}
		}
		
		if(accel != Vector2.zero) {
			velocity += accel*Time.deltaTime;
		}
		
		if(maxVelocity > 0) {
			if(velocity.y == 0 && velocity.x > maxVelocity) {
				velocity.x = maxVelocity;
			}
			else if(velocity.sqrMagnitude > mMaxVelocitySq) {
				velocity.Normalize();
				velocity *= maxVelocity;
			}
		}
		
		if(velocity != Vector2.zero || mYVel != 0) {
			planetPos += new Vector2(velocity.x*dt, (velocity.y+mYVel)*dt);
		}
		
		//convert to world space
		PolarCoord polarPos = new PolarCoord(planetPos.y + planet.radius, (planetPos.x/planet.surfaceLength)*PolarCoord.PI_2);
		
		Vector3 pos = mTrans.position;
		Vector3 nPos = _planetPos + polarPos.ToVector3(); nPos.z = pos.z;
		mTrans.position = nPos;
	}
}
