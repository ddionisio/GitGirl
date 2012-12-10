using UnityEngine;
using System.Collections;

public class Entity : EntityBase {
	public const float collisionCastZ = -1000.0f;
	public const float collisionDistance = 2000.0f;
	
	public enum Action {
		idle,
		start, //used by scene for player
		spawning, //once finish, calls OnEntitySpawnFinish to listeners
		reviving,
		revived,
		hurt,
		die,
		move,
		attack,
		jump,
		grabbed,
		stunned,
		victory,
		jump_fall,
		fall,
		mad, //cutscene purpose
		
		NumActions,
		
		none = NumActions
	}
	
	
	
	public interface IListener {
		void OnEntityAct(Action act);
		void OnEntityInvulnerable(bool yes);
		
		//youAreReceiver=true is when you cast the ray, other=null for non-entities
		void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver);
		
		void OnEntitySpawnFinish();
	}
	
	public Action sceneStartAction = Action.NumActions; //only use this if placing entities on level manually, otherwise just put it in numactions
	
	public float spawnDelay = 1.0f;
	
	protected int mCollideLayerMask = 0; //0 is none, only initialize on start	
		
	private Action mCurAct = Action.NumActions;
	private Action mPrevAct = Action.NumActions;
	
	private EntityStats mStats;
	private PlanetAttach mPlanetAttach;
	
	private float mEntCurTime = 0;
	
	private float mInvulDelay = 0;
	
	private IListener[] mListeners;
	
	private AIState mAIStateInstance = null;
	private string mAICurState;
	//private bool mAIHasStarted = false;
	
	//AI
	public void AIStop() {
		if(mAIStateInstance != null) {
			mAIStateInstance.terminate = true;
			mAIStateInstance = null;
			
			mAICurState = null;
		}
	}
	
	public void AISetPause(bool pause) {
		if(mAIStateInstance != null) {
			mAIStateInstance.pause = pause;
		}
	}
	
	public void AISetState(string state) {
		AIStop();
		
		mAICurState = state;
		
		mAIStateInstance = new AIState();
		AIManager.instance.states.Start(this, mAIStateInstance, state);
	}
	
	public void AIRestart() {
		if(!string.IsNullOrEmpty(mAICurState)) {
			AISetState(mAICurState);
		}
	}
	
	void AIChangeState(string state) {
		AISetState(state);
	}
	
	
	public string aiCurState {
		get {
			return mAICurState;
		}
	}
	
	public bool aiActive {
		get {
			return mAIStateInstance != null;
		}
	}
	
	//
	
	public EntityStats stats {
		get {
			return mStats;
		}
	}
	
	public PlanetAttach planetAttach {
		get {
			return mPlanetAttach;
		}
	}
	
	public Action prevAction {
		get {
			return mPrevAct;
		}
	}
	
	public Action action {
		get {
			return mCurAct;
		}
		set {
			if(mCurAct != value) {
				mPrevAct = mCurAct;
				mCurAct = value;
				if(mCurAct != Action.NumActions) {
					foreach(IListener l in mListeners) {
						l.OnEntityAct(value);
					}
				}
			}
		}
	}
	
	public void YieldSetAction(Action act) {
		StartCoroutine(OnYieldAction(act));
	}
	
	IEnumerator OnYieldAction(Action act) {
		yield return new WaitForFixedUpdate();
		
		action = act;
		
		yield break;
	}
	
	public void InvulnerableOff() {
		mEntCurTime = 0;
		
		FlagsRemove(Flag.Invulnerable);
		
		foreach(IListener l in mListeners) {
			l.OnEntityInvulnerable(false);
		}
	}
	
	public void Invulnerable(float delay) {
		mEntCurTime = 0;
		mInvulDelay = delay;
		FlagsAdd(Flag.Invulnerable);
		
		foreach(IListener l in mListeners) {
			l.OnEntityInvulnerable(true);
		}
	}
	
	public virtual bool CanHarmPlayer() {
		return false;
	}
	
	/// <summary>
	/// Spawn this entity, resets stats, set action to spawning, then later calls OnEntitySpawnFinish.
	/// NOTE: calls after an update to ensure Awake and Start is called.
	/// </summary>
	public void Spawn() {
		mCurAct = mPrevAct = Action.NumActions; //avoid invalid updates
		//ensure start is called before spawning if we are freshly allocated from entity manager
		StartCoroutine(DoSpawn());
	}
	
	public void Release() {
		AIStop();
		StopAllCoroutines();
		EntityManager.instance.Release(transform);
	}
	
	//////////internal methods
	
	
			
	/////////////implements
	
	protected virtual void OnDestroy() {
		mAIStateInstance = null;
		mListeners = null;
	}
	
	protected virtual void Awake() {
		mStats = GetComponent<EntityStats>();
		mPlanetAttach = GetComponent<PlanetAttach>();
		
		Component[] cs = GetComponentsInChildren(typeof(IListener), true);
		mListeners = new IListener[cs.Length];
		for(int i = 0; i < cs.Length; i++) {
			mListeners[i] = cs[i] as IListener;
		}
	}
	
	protected virtual void OnEnable() {
	}
	
	protected virtual void Start() {
		action = sceneStartAction;
	}
	
	void Update() {
		switch(mCurAct) {
		case Action.NumActions:
			//why are we here?
			break;
			
		case Action.spawning:
			mEntCurTime += Time.deltaTime;
			if(mEntCurTime >= spawnDelay) {
				mCurAct = Action.NumActions; //no act until set later
				
				foreach(IListener l in mListeners) {
					l.OnEntitySpawnFinish();
				}
			}
			break;
			
		default:
			//check collision
			//planet attach necessary?
			if(mListeners.Length > 0 && mCollideLayerMask > 0 && mPlanetAttach != null) {
				float radius = mPlanetAttach.radius;
				RaycastHit hit;
				Vector3 castPos = transform.position; castPos.z = collisionCastZ;
				if(Physics.SphereCast(castPos, radius, Vector3.forward, out hit, collisionDistance, mCollideLayerMask)) {
					Entity e = hit.transform.GetComponent<Entity>();
					
					foreach(IListener l in mListeners) {
						l.OnEntityCollide(e, hit, true);
					}
					
					if(e != null) {
						//tell the other receiving end
						foreach(IListener lOther in e.mListeners) {
							lOther.OnEntityCollide(this, hit, false);
						}
					}
				}
			}
			
			if(FlagsCheck(Flag.Invulnerable)) {
				mEntCurTime += Time.deltaTime;
				if(mEntCurTime >= mInvulDelay) {
					InvulnerableOff();
				}
			}
			break;
		}
	}
	
	//////////internal
		
	IEnumerator DoSpawn() {
		yield return new WaitForFixedUpdate();
		
		if(stats != null) {
			stats.ResetStats();
		}
		
		mEntCurTime = 0.0f;
		
		action = Action.spawning;
		
		yield break;
	}
}
