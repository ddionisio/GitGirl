using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public const int maxJump = 2;
	
	public float airControlAccel;
	
	private Player mPlayer;
	private PlayerGrabber mGrabber;
	
	void Awake() {
		mPlayer = GetComponent<Player>();
		mGrabber = GetComponent<PlayerGrabber>();
	}
	
	void Update() {
		PlanetAttach planetAttach = mPlayer.planetAttach;
		
		float xS = Input.GetAxis("Horizontal");
		if(xS > 0.0f) {
			xS = -1.0f;
		}
		else if(xS < 0.0f) {
			xS = 1.0f;
		}
		
		if(planetAttach.jumpCounter < maxJump) {
			if(Input.GetButtonDown("Jump")) {
				mPlayer.action = Entity.Action.jump;
				
				planetAttach.Jump(mPlayer.jumpSpeed);
				
				if(xS != 0.0f) {
					planetAttach.velocity.x = xS*mPlayer.moveSpeed;
				}
			}
		}
		
		if(planetAttach.jumpCounter == 0 && planetAttach.isGround) {
			if(xS == 0) {
				if(mPlayer.action == Entity.Action.move || mPlayer.action == Entity.Action.jump) {
					mPlayer.action = Entity.Action.idle;
					
					planetAttach.velocity.x = 0;
				}
			}
			else {
				mPlayer.action = Entity.Action.move;
				
				planetAttach.velocity.x = xS*mPlayer.moveSpeed;
			}
			
			planetAttach.accel = Vector2.zero;
		}
		else {
			planetAttach.accel.x = xS*airControlAccel;
		}
		
		if(Input.GetButtonDown("Fire")) {
			mGrabber.Fire(true);
		}
		else if(Input.GetButtonUp("Fire")) {
			mGrabber.Fire(false);
		}
		else if(Input.GetButtonDown("Expel")) {
			mGrabber.Expel();
		}
	}
}
