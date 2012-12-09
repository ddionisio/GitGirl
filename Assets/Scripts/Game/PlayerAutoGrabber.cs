using UnityEngine;
using System.Collections;

public class PlayerAutoGrabber : PlayerGrabberBase {
	public tk2dBaseSprite sprite;
	
	public void Activate() {
		SwitchState(PlayerGrabberBase.State.None);
	}
	
	protected override void Start() {
		base.Start();
	}
	
	protected override void SwitchState(State newState) {
		base.SwitchState(newState);
		
		switch(newState) {
		case State.None:
			sprite.scale = new Vector3(1.0f, 0.0f, 1.0f);
			break;
			
		case State.Grabbing:
			player.RefreshAutoGrabber();
			break;
			
		case State.Retracted:
			gameObject.SetActiveRecursively(false);
			player.RefreshAutoGrabber();
			break;
		}
	}
	
	void Awake() {
	}
	
	void Update() {
		switch(state) {
		case PlayerGrabber.State.None:
			//look for a grabbable entity
			RaycastHit hit;
			Vector3 pos = transform.position; pos.z = Entity.collisionCastZ;
			if(Physics.SphereCast(pos, radius, Vector3.forward, out hit, Entity.collisionDistance, Main.layerMaskAutoGrab)) {
				//only valid for entities
				EntityBase ent = hit.transform.GetComponentInChildren<EntityBase>();
				if(ent != null) {
					Grab(hit.transform);
				}
			}
			break;
			
		case PlayerGrabber.State.Grabbing:
			GrabbingUpdate(true, false);
			break;
			
		case PlayerGrabber.State.Retracting:
			GrabbingUpdate(true, true);
			break;
		}
	}
	
	private void GrabbingUpdate(bool isMotion, bool isRetract) {
		mGrabCurDelay += Time.deltaTime;
		
		Vector3 neckPos = transform.position;
		
		Vector2 dir = (isRetract || mGrabTarget == null ? mGrabDest : mGrabTarget.position) - neckPos;
		float len = dir.magnitude;
		dir /= len;
		
		len -= grabLenOfs;
		
		bool done = mGrabCurDelay >= grabDelay || !isMotion;
		
		float curLen;
		if(isRetract) {
			curLen = done ? 0.0f : Ease.Out(mGrabCurDelay, grabDelay, len, -len);
		}
		else {
			curLen = done ? len : Ease.In(mGrabCurDelay, grabDelay, 0.0f, len);
		}
		
		transform.up = dir;
		
		//move attacher
		Vector3 headPos = headAttach.position;
		Vector2 newHeadPos = neckPos;
		newHeadPos += dir*curLen;
		headAttach.position = new Vector3(newHeadPos.x, newHeadPos.y, headPos.z);
		
		//properly scale neck
		sprite.scale = Vector3.one;
		Bounds neckBounds = sprite.GetBounds();
		Vector3 newNeckScale = Vector3.one;
		newNeckScale.y = curLen/neckBounds.size.y;
		sprite.scale = newNeckScale;
		
		if(isMotion && done) {
			SwitchState(isRetract ? State.Retracted : State.Grabbed);
		}
	}
	
	private void Grab(Transform target) {
		mGrabTarget = target;
		
		SwitchState(PlayerGrabber.State.Grabbing);
	}
}
