using UnityEngine;
using System.Collections;

public class PlayerGrabber : MonoBehaviour {
	public enum State {
		None,
		Grabbing,
		Grabbed,
		Retracting,
		Retracted,
		Holding //holding an object
	}
	
	public Transform player;
	
	public Transform head;
	public Transform headAttach; //attachment point
	
	public string headClipIdle;
	public string headClipGrab;
	public string headClipHold;
	public string headClipThrow;
	
	public Transform neck;
	
	public float radius = 10.0f;
	
	public float lookAngleLimit = 90.0f;
	
	public float grabLenOfs = 0.0f;
	public float grabDelay = 0.15f;
	
	//cache
	public int mLayerMasksGrab;
	
	private Player mPlayer;
	private tk2dAnimatedSprite mHeadSprite;
	private tk2dSprite mNeckSprite;
		
	//stuff
	private State mCurState = State.None;
	
	private float mGrabCurDelay = 0.0f;
	
	private Transform mGrabTarget = null;
	
	private bool mRetractIsAttached;
	private Vector3 mGrabDest;
	
	private bool mDisable = false;
	private bool mSceneDisable = false;
	
	private int mHeadClipIdleId;
	private int mHeadClipGrabId;
	public int mHeadClipHoldId;
	public int mHeadClipThrowId;
	
	private Reticle mReticleCheck;
	
	public State state {
		get {
			return mCurState;
		}
	}
	
	public Player thePlayer {
		get {
			return mPlayer;
		}
	}
	
	public void Retract(bool attachGrabbedTarget) {
		mRetractIsAttached = attachGrabbedTarget;
		SwitchState(State.Retracting);
	}
	
	//
	//
	
	/// <summary>
	/// You better know what to do with the grabbed item, parent is set to null.
	/// e.g. you can move it back to a pool of some sort.
	/// </summary>
	/// <returns>
	/// The detached object, parentless. :(
	/// </returns>
	public Transform DetachGrab() {
		SwitchState(State.None);
		
		Transform ret = mGrabTarget;
		if(ret != null) {
			ret.parent = null;
			
			ret.SendMessage("OnGrabDetach", this, SendMessageOptions.DontRequireReceiver);
		}
		
		mGrabTarget = null;
		mRetractIsAttached = false;
						
		return ret;
	}
	
	void Awake() {
		mPlayer = player.GetComponent<Player>();
		
		mHeadSprite = head.GetComponent<tk2dAnimatedSprite>();
		mHeadClipIdleId = mHeadSprite.GetClipIdByName(headClipIdle);
		mHeadClipGrabId = mHeadSprite.GetClipIdByName(headClipGrab);
		mHeadClipHoldId = mHeadSprite.GetClipIdByName(headClipHold);
		mHeadClipThrowId = mHeadSprite.GetClipIdByName(headClipThrow);
		
		mNeckSprite = neck.GetComponent<tk2dSprite>();
	}
		
	// Use this for initialization
	void Start () {
		//detect planet to avoid grabbing into the ground
		mLayerMasksGrab = Main.layerMaskEnemy | Main.layerMaskProjectile | Main.layerMaskItem | Main.layerMaskPlanet;
		
		//Input.mousePosition
		SwitchState(State.None);
	}
	
	void OrientHead(Vector2 dir, bool lockAngle) {
		Vector2 playerUp = player.transform.up;
		
		float side = Util.Vector2DCross(playerUp, dir) < 0 ? -1 : 1;
		
		float angle = Mathf.Acos(Vector2.Dot(playerUp, dir));
		
		float limitAngle = lookAngleLimit*Mathf.Deg2Rad;
		
		if(lockAngle && angle > limitAngle) {
			dir = Util.Vector2DRot(playerUp, -side*limitAngle);
		}
		
		Vector3 headScale = mHeadSprite.scale;
		headScale.x = side*Mathf.Abs(headScale.x);
		mHeadSprite.scale = headScale;
		
		head.rotation = Quaternion.FromToRotation(head.up, dir) * head.rotation;
	}
	
	void LookAtMouse() {
		Vector2 mouse = Util.MouseToScreen();
		
		Vector2 headPos = head.position;
		
		Vector2 dir = mouse - headPos;
		dir.Normalize();
		
		OrientHead(dir, true);
	}
	
	void RefreshReticles() {
		Vector2 mouse = Util.MouseToScreen();
		
		Vector2 headPos = head.position;
		
		Vector2 dir = mouse - headPos;
		float len = dir.magnitude;
		dir /= len;
		
		if(len > radius) {
			len = radius;
		}
		
		//Main.instance.reticleManager.ActivateInRange(head.position, radius, mLayerMasksGrab);
						
		RaycastHit hit;
		if(Physics.Raycast(headPos, dir, out hit, len, mLayerMasksGrab)) {
			if(mReticleCheck == null || hit.transform != mReticleCheck.transform.parent) {
				Main.instance.reticleManager.DeactivateAll();
				mReticleCheck = Main.instance.reticleManager.Activate(hit.transform);
			}
			else if(mReticleCheck.entity == null || mReticleCheck.entity.reticle == Reticle.Type.NumType) {
				Main.instance.reticleManager.DeactivateAll();
				mReticleCheck = null;
			}
		}
		else {
			Main.instance.reticleManager.DeactivateAll();
			mReticleCheck=null;
		}
	}
	
	void GrabThrow() {
		if(!(mDisable || mSceneDisable) && Input.GetButtonDown("Fire1")) {
			Transform t = DetachGrab();
			
			mHeadSprite.Play(mHeadClipThrowId);
			
			mPlayer.OnGrabThrow();
			t.SendMessage("OnGrabThrow", this, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void GrabFromMouse() {
		if(!(mDisable || mSceneDisable) && Input.GetButtonDown("Fire1")) {
			Main main = Main.instance;
			Transform target = main.reticleManager.GetTarget();
			if(target != null) {
				Main.instance.reticleManager.DeactivateAll();
				mGrabTarget = target;
				SwitchState(State.Grabbing);
			}
		}
	}
	
	void GrabbingUpdate(bool isMotion, bool isRetract) {
		mGrabCurDelay += Time.deltaTime;
		
		Vector3 neckPos = neck.position;
		
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
		
		OrientHead(dir, false);
		
		Vector3 headPos = head.position;
		Vector2 newHeadPos = neckPos;
		newHeadPos += dir*curLen;
		head.position = new Vector3(newHeadPos.x, newHeadPos.y, headPos.z);
		
		//orient neck to head
		neck.rotation = head.rotation;
		
		//properly scale neck
		mNeckSprite.scale = Vector3.one;
		Bounds neckBounds = mNeckSprite.GetBounds();
		Vector3 newNeckScale = Vector3.one;
		newNeckScale.y = curLen/neckBounds.size.y;
		mNeckSprite.scale = newNeckScale;
		
		if(isMotion && done) {
			SwitchState(isRetract ? State.Retracted : State.Grabbed);
		}
	}
	
	void SwitchState(State newState) {
		mCurState = newState;
		
		switch(newState) {
		case State.None:
			mHeadSprite.Play(mHeadClipIdleId);
			break;
			
		case State.Grabbing:
			mGrabCurDelay = 0.0f;
			
			Main.instance.reticleManager.DeactivateAll(mGrabTarget);
			
			mPlayer.OnGrabStart();
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabStart", this, SendMessageOptions.DontRequireReceiver);
			}
			
			mHeadSprite.Play(mHeadClipGrabId);
			break;
			
		case State.Grabbed:
			Main.instance.reticleManager.DeactivateAll();
			
			mPlayer.OnGrabDone();
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabDone", this, SendMessageOptions.DontRequireReceiver);
			}
			
			//wait for action on grabbed target
			break;
			
		case State.Retracting:
			mGrabCurDelay = 0.0f;
			
			if(mGrabTarget != null) {
				mGrabDest = mGrabTarget.position;
				
				//move grabbed object to attach point
				if(mRetractIsAttached) {
					mGrabTarget.parent = headAttach;
					mGrabTarget.localPosition = new Vector3(0, 0, mGrabDest.z);
					mGrabTarget.localScale = Vector3.one;
					mGrabTarget.localRotation = Quaternion.identity;
				}
			}
			else {
				mGrabDest = transform.position;
			}
			
			mPlayer.OnGrabRetractStart();
			
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabRetractStart", this, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case State.Retracted:
			mPlayer.OnGrabRetractEnd();
			
			if(mRetractIsAttached && mGrabTarget != null) { //don't care if grabbed is left alone
				mGrabTarget.SendMessage("OnGrabRetractEnd", this, SendMessageOptions.DontRequireReceiver);
			}
			else {
				mGrabTarget = null;
			}
			
			SwitchState(State.None);
			break;
		}
	}
	
	void Update () {
		switch(mCurState) {
		case State.None:
			LookAtMouse();
			
			if(mGrabTarget == null) {
				if(!(mHeadSprite.clipId == mHeadClipIdleId || mHeadSprite.clipId == mHeadClipThrowId)) {
					mHeadSprite.Play(mHeadClipIdleId);
				}
				
				RefreshReticles();
				GrabFromMouse();
			}
			else {
				if(mHeadSprite.clipId != mHeadClipHoldId) {
					mHeadSprite.Play(mHeadClipHoldId);
				}
				
				GrabThrow();
			}
			break;
			
		case State.Holding:
			LookAtMouse();
			break;
			
		case State.Grabbing:
			GrabbingUpdate(true, false);
			break;
			
		case State.Grabbed:
			GrabbingUpdate(false, false);
			break;
			
		case State.Retracting:
			GrabbingUpdate(true, true);
			break;
		}
	}
	
	void OnPlayerDeath() {
		neck.gameObject.SetActiveRecursively(false);
		head.gameObject.SetActiveRecursively(false);
	}
	
	void OnUIModalActive() {
		mDisable = true;
	}
	
	void OnUIModalInactive() {
		mDisable = false;
	}
	
	void OnSceneActivate(bool activate) {
		mSceneDisable = !activate;
	}
}
