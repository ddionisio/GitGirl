using UnityEngine;
using System.Collections;

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
	
	public Transform head;
	
	public ThrowCallback throwCallback=null;
	
	private PlayerController mController;
	
	private float mPlayerCurTime;
	
	private PlayerStats mPlayerStats;
	
	private bool mSceneDisable = false;
	
	private SceneLevel.LevelCheckpoint mCheckPoint = null;
	
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
	}
	
	protected override void Start () {
		base.Start();
		
		if(mCheckPoint != null) {
			action = Entity.Action.idle;
			
			mPlayerStats.score = mCheckPoint.playerScore;
			
			planetAttach.planetPos = mCheckPoint.playerPlanetPos;
			transform.position = planetAttach.planet.ConvertToWorldPos(mCheckPoint.playerPlanetPos);
			
			Main.instance.cameraController.attach = transform;
			Main.instance.cameraController.mode = CameraController.Mode.Attach;
			Main.instance.cameraController.CancelMove();
			
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
				
	public void OnGrabStart() {
		mController.enabled = false;
	}
	
	public void OnGrabDone() {
		mController.enabled = true;
	}
	
	public void OnGrabRetractStart() {
	}
	
	public void OnGrabRetractEnd() {
	}
	
	public void OnGrabThrow() {
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
		if(prevAction == Entity.Action.start) {
			head.gameObject.SetActiveRecursively(true);
		}
		
		switch(act) {
		case Action.start:
			head.gameObject.SetActiveRecursively(false);
			break;
			
		case Action.idle:
			//revived!
			if(prevAction == Entity.Action.die) {
				Invulnerable(hurtInvulDelay);
				mController.enabled = true;
			}
			
			planetAttach.velocity = Vector2.zero;
			break;
			
		case Action.hurt:
			_SetActionDisablePlayer();
			Invulnerable(hurtInvulDelay);
			break;
			
		case Action.die:
			_SetActionDisablePlayer();
			BroadcastMessage("OnPlayerDeath", null, SendMessageOptions.DontRequireReceiver);
			break;
			
		case Action.victory:
			_SetActionDisablePlayer();
			break;
		}
	}
	
	void _SetActionDisablePlayer() {
		mPlayerCurTime = 0.0f;
		planetAttach.velocity = Vector2.zero;
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
	
	public void OnEntityCollide(Entity other, bool youAreReceiver) {
		if(youAreReceiver && other.CanHarmPlayer()) {
			stats.ApplyDamage(other.stats);
			if(stats.curHP == 0) {
				//dead!
				action = Entity.Action.die;
				
				planetAttach.velocity = Vector2.zero;
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
