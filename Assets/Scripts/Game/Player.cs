using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity, Entity.IListener {
	public delegate void ThrowCallback(Player player);
	
	public float hurtInvulDelay;
	public float hurtDelay;
	
	/// <summary> The speed at which we get knocked back when hurt. </summary>
	public float hurtSpeed;
	/// <summary> The speed at which we jump when we get hurt. </summary>
	public float hurtJumpSpeed;
	
	/// <summary> For moving left/right (angle/s) </summary>
	public float moveSpeed;
	
	/// <summary> For jumping (m/s) </summary>
	public float jumpSpeed;
	
	public float deathDelay = 2.0f; //delay to bring game over menu up
	
	//auto grabbing stuff
	public int maxAutoGrab = 5;
	public Transform autoGrabHolder;
	public PlayerAutoGrabber autoGrabTemplate;
	
	public ThrowCallback throwCallback=null;
	
	private PlayerController mController;
	
	private float mPlayerCurTime;
	
	private PlayerStats mPlayerStats;
	
	private bool mSceneDisable = false;
	
	private SceneLevel.LevelCheckpoint mCheckPoint = null;
	
	private Queue<PlayerAutoGrabber> mAutoGrabQueue;
	
	//only called by level scene
	public void SetCheckpoint(SceneLevel.LevelCheckpoint checkpoint) {
		mCheckPoint = checkpoint;
	}
	
	///// implements
	
	protected override void Awake() {
		base.Awake();
		
		SetDefaultCollideMask();
		
		if(stats != null) {
			mPlayerStats = stats as PlayerStats;
		}
		
		mController = GetComponent<PlayerController>();
		
		mAutoGrabQueue = new Queue<PlayerAutoGrabber>(maxAutoGrab);
		for(int i = 0; i < maxAutoGrab; i++) {
			PlayerAutoGrabber newGrabber = (PlayerAutoGrabber)Object.Instantiate(autoGrabTemplate);
			Transform t = newGrabber.transform;
			t.parent = autoGrabHolder;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			
			//leave the first one active
			if(i > 0) {
				newGrabber.gameObject.SetActiveRecursively(false);
			}
			
			mAutoGrabQueue.Enqueue(newGrabber);
		}
	}
	
	protected override void Start () {
		base.Start();
		
		if(mCheckPoint != null) {
			action = Entity.Action.idle;
			
			mPlayerStats.score = mCheckPoint.playerScore;
			
			planetAttach.planetPos = mCheckPoint.playerPlanetPos;
			transform.position = planetAttach.planet.ConvertToWorldPos(mCheckPoint.playerPlanetPos);
			
			CameraController.instance.attach = transform;
			CameraController.instance.mode = CameraController.Mode.Attach;
			CameraController.instance.CancelMove();
			
			mCheckPoint = null;
		}
	}
	
	protected override void OnEnable() {
		base.OnEnable();
	}
	
	protected override void OnDestroy () {
		base.OnDestroy ();
		
		mCheckPoint = null;
		throwCallback = null;
	}
	
	void SetDefaultCollideMask() {
		mCollideLayerMask = Main.layerMaskEnemy | Main.layerMaskEnemyComplex 
			| Main.layerMaskEnemyNoPlayerProjectile | Main.layerMaskProjectile;
	}
				
	public void OnGrabStart(PlayerGrabberBase grabber) {
		if(grabber is PlayerGrabber)
			mController.enabled = false;
	}
	
	public void OnGrabDone(PlayerGrabberBase grabber) {
		if(grabber is PlayerGrabber)
			mController.enabled = true;
	}
	
	public void OnGrabRetractStart(PlayerGrabberBase grabber) {
	}
	
	public void OnGrabRetractEnd(PlayerGrabberBase grabber) {
	}
	
	public void OnGrabThrow(PlayerGrabberBase grabber) {
		if(throwCallback != null) {
			throwCallback(this);
		}
	}
	
	public void AddScore(int amt) {
		if(mPlayerStats != null && amt > 0) {
			mPlayerStats.AddScore(amt);
		}
	}
	
	public int GetScore() {
		return mPlayerStats != null ? mPlayerStats.score : 0;
	}
	
	public void RefreshAutoGrabber() {
		if(mAutoGrabQueue.Count > 0) {
			PlayerAutoGrabber autoGrabber = mAutoGrabQueue.Peek();
			
			if(!autoGrabber.gameObject.active) {
				autoGrabber.gameObject.SetActiveRecursively(true);
				autoGrabber.Activate();
			}
			else if(autoGrabber.state != PlayerGrabberBase.State.None) {
				//still in progress, move back and activate the next if it hasn't yet
				mAutoGrabQueue.Dequeue();
				mAutoGrabQueue.Enqueue(autoGrabber);
				
				autoGrabber = mAutoGrabQueue.Peek();
				if(!autoGrabber.gameObject.active) {
					autoGrabber.gameObject.SetActiveRecursively(true);
					autoGrabber.Activate();
				}
			}
		}
	}
	
	///// internal
	
	
	// Update is called once per frame
	void LateUpdate () {
		switch(action) {
		case Action.hurt:
			mPlayerCurTime += Time.deltaTime;
			if(mPlayerCurTime >= hurtDelay) {
				mController.enabled = true;
				action = Entity.Action.idle;
			}
			break;
			
		case Action.die:
			mPlayerCurTime += Time.deltaTime;
			if(mPlayerCurTime >= deathDelay) {
				UIManager.instance.ModalOpen(UIManager.Modal.GameOver);
			}
			break;
		}
	}
	
	void OnUIModalActive() {
		if(!mSceneDisable) { //let scene activate
			planetAttach.velocity.x = 0;
			mController.enabled = false;
		}
	}
	
	void OnUIModalInactive() {
		if(!mSceneDisable) { //let scene activate
			mController.enabled = true;
		}
	}
	
	public void OnEntityAct(Action act) {
		switch(act) {
		case Action.start:
			break;
			
		case Action.idle:
			//revived!
			if(prevAction == Entity.Action.die) {
				Invulnerable(hurtInvulDelay);
				mController.enabled = true;
			}
			
			planetAttach.ResetMotion();
			break;
			
		case Action.hurt:
			_SetActionDisablePlayer();
			Invulnerable(hurtInvulDelay);
			break;
			
		case Action.die:
			_SetActionDisablePlayer();
			break;
			
		case Action.victory:
			_SetActionDisablePlayer();
			break;
		}
	}
	
	void _SetActionDisablePlayer() {
		mPlayerCurTime = 0.0f;
		planetAttach.ResetMotion();
		mController.enabled = false;
	}
	
	public void OnEntityInvulnerable(bool yes) {
		if(yes) {
			mCollideLayerMask = 0;
		}
		else {
			SetDefaultCollideMask();
		}
	}
	
	public void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver) {
		if(other != null) {
			if(youAreReceiver && other.CanHarmPlayer()) {
				stats.ApplyDamage(other.stats);
				if(stats.curHP == 0) {
					//dead!
					action = Entity.Action.die;
					
					planetAttach.ResetMotion();
				}
				else {
					//get hurt
					action = Entity.Action.hurt;
					
					//knock back
					switch(planetAttach.CheckSide(other.planetAttach)) {
					case Util.Side.Left:
					case Util.Side.None:
						planetAttach.velocity = new Vector2(hurtSpeed, 0);
						break;
					case Util.Side.Right:
						planetAttach.velocity = new Vector2(-hurtSpeed, 0);
						break;
					}
					
					planetAttach.Jump(hurtJumpSpeed, false);
				}
			}
		}
		else { //TODO: something for platform, etc.
		}
	}
	
	public void OnEntitySpawnFinish() {
		//players don't really spawn...unless we implement lives, or restart, or whatever
	}
	
	void OnSceneActivate(bool activate) {
		mSceneDisable = !activate;
		
		mController.enabled = activate;
		planetAttach.applyGravity = activate;
		
		if(!activate) {
			planetAttach.ResetCurYVel();
			action = Entity.Action.idle;
		}
	}
}
