using UnityEngine;
using System.Collections;

public class PlayerGrabber : PlayerGrabberBase, Entity.IListener {
	[SerializeField] Transform head;
	
	public string headClipIdle;
	public string headClipGrab;
	public string headClipHold;
	public string headClipThrow;
	public string headClipFireHold;
	
	public Transform neck;
	
	public float lookAngleLimit = 90.0f;
					
	public Weapon[] weapons;
	
	//cache
	private tk2dAnimatedSprite mHeadSprite;
	private tk2dSprite mNeckSprite;
		
	//stuff
	private int mHeadClipIdleId;
	private int mHeadClipGrabId;
	private int mHeadClipHoldId;
	private int mHeadClipThrowId;
	private int mHeadClipFireHoldId;
	
	private Reticle mReticleCheck;
	
	private Weapon mCurWeapon = null;
	
	private int mLayerMasksGrab;
	
	public override Vector3 up {
		get {
			return head.up;
		}
	}
	
	public tk2dAnimatedSprite headSprite {
		get { return mHeadSprite; }
	}
	
	public void Equip(Weapon.Type type) {
		Expel(); //just in case
		
		if(type != Weapon.Type.NumTypes) {
			mCurWeapon = weapons[(int)type];
			mCurWeapon.gameObject.SetActiveRecursively(true);
			mCurWeapon.Equip();
		}
	}
	
	//expel current weapon
	public void Expel() {
		if(mCurWeapon != null) { 
			mCurWeapon.Expel();
			mCurWeapon.gameObject.SetActiveRecursively(false);
			mCurWeapon = null;
			
			PlayAnimIdle();
		}
	}
	
	//do something
	public void Fire(bool down) {
		if(state == State.None) {
			if(mCurWeapon != null) {
				mCurWeapon.Fire(down);
			}
			else if(mGrabTarget != null) {
				if(down) {
					GrabThrow();
				}
			}
			else {
				if(down) {
					GrabFromMouse();
				}
			}
		}
	}
	
	public void PlayAnimThrow() {
		mHeadSprite.Play(mHeadClipThrowId);
	}
	
	public void PlayAnimFireHold() {
		mHeadSprite.Play(mHeadClipFireHoldId);
	}
	
	public void PlayAnimIdle() {
		if(mGrabTarget == null && mCurWeapon == null) {
			mHeadSprite.Play(mHeadClipIdleId);
		}
		else {
			mHeadSprite.Play(mHeadClipHoldId);
		}
	}
	
	//
	//
	
	void HeadAnimComplete(tk2dAnimatedSprite sprite, int clipId) {
		if(clipId == mHeadClipThrowId) {
			PlayAnimIdle();
		}
	}
	
	void Awake() {
		mHeadSprite = head.GetComponent<tk2dAnimatedSprite>();
		mHeadClipIdleId = mHeadSprite.GetClipIdByName(headClipIdle);
		mHeadClipGrabId = mHeadSprite.GetClipIdByName(headClipGrab);
		mHeadClipHoldId = mHeadSprite.GetClipIdByName(headClipHold);
		mHeadClipThrowId = mHeadSprite.GetClipIdByName(headClipThrow);
		mHeadClipFireHoldId = mHeadSprite.GetClipIdByName(headClipFireHold);
		
		mHeadSprite.animationCompleteDelegate = HeadAnimComplete;
		
		mNeckSprite = neck.GetComponent<tk2dSprite>();
		
		neck.gameObject.SetActiveRecursively(false);
		
		foreach(Weapon weapon in weapons) {
			weapon.Init(this);
			weapon.gameObject.SetActiveRecursively(false);
		}
	}
		
	// Use this for initialization
	protected override void Start () {
		//detect planet to avoid grabbing into the ground
		mLayerMasksGrab = Main.layerMaskEnemy | Main.layerMaskProjectile | Main.layerMaskItem | Main.layerMaskPlanet;
		
		base.Start();
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
		Transform t = DetachGrab();
		
		PlayAnimThrow();
		
		player.OnGrabThrow(this);
		t.SendMessage("OnGrabThrow", this, SendMessageOptions.DontRequireReceiver);
	}
	
	void GrabFromMouse() {
		Main main = Main.instance;
		Transform target = main.reticleManager.GetTarget();
		if(target != null) {
			Main.instance.reticleManager.DeactivateAll();
			mGrabTarget = target;
			SwitchState(State.Grabbing);
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
		
	protected override void SwitchState(State newState) {		
		switch(newState) {
		case State.None:
			PlayAnimIdle();
			break;
			
		case State.Grabbing:
			neck.gameObject.SetActiveRecursively(true);
			
			Main.instance.reticleManager.DeactivateAll(mGrabTarget);
			
			mHeadSprite.Play(mHeadClipGrabId);
			break;
			
		case State.Grabbed:
			Main.instance.reticleManager.DeactivateAll();
			break;
			
		case State.Retracting:
			break;
			
		case State.Retracted:
			neck.gameObject.SetActiveRecursively(false);
			break;
		}
		
		base.SwitchState(newState);
	}
	
	void Update () {
		switch(state) {
		case State.None:
			LookAtMouse();
			
			if(mGrabTarget == null && mCurWeapon == null) {
				RefreshReticles();
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
	
	void OnUIModalActive() {
	}
	
	void OnUIModalInactive() {
		if(mCurWeapon != null) {
			mCurWeapon.Fire(false);
		}
	}
	
	void OnSceneActivate(bool activate) {
		if(mCurWeapon != null && !activate) {
			mCurWeapon.Fire(false);
		}
	}
	
	//entity listener
	
	public void OnEntityAct(Entity.Action act) {
		if(player.prevAction == Entity.Action.start) {
			head.gameObject.active = true;
			headAttach.gameObject.SetActiveRecursively(true);
		}
		
		switch(act) {
		case Entity.Action.start:
			head.gameObject.active = false;
			headAttach.gameObject.SetActiveRecursively(false);
			break;
			
		case Entity.Action.die:
			neck.gameObject.SetActiveRecursively(false);
			head.gameObject.SetActiveRecursively(false);
			
			Expel();
			break;
		}
	}
	
	public void OnEntityInvulnerable(bool yes) {
	}
	
	public void OnEntityCollide(Entity other, bool youAreReceiver) {
	}
	
	public void OnEntitySpawnFinish() {
	}
}
