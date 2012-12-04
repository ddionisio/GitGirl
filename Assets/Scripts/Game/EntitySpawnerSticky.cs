using UnityEngine;
using System.Collections;

//respawn after entity has been claimed by entity manager
public class EntitySpawnerSticky : MonoBehaviour {
	
	public string type;
	
	public float delayRespawn = 1.0f;
	
	public bool activeOnStart = false;
	
	public bool useSpawnFX = false;
	
	enum State {
		Inactive,
		Spawn,
		SpawnWait
	}
	
	private State mCurState = State.Inactive;
	private float mCurTime = 0;
	private PoolDataController mPoolController = null;
	private bool mSceneActive = true;
	
	public void Activate(bool yes) {
		if(yes) {
			ChangeState(State.SpawnWait);
		}
		else {
			ChangeState(State.Inactive);
		}
	}
	
	void Start() {
		if(activeOnStart && mSceneActive) {
			Activate(true);
		}
	}
	
	void OnSceneActivate(bool yes) {
		mSceneActive = yes;
		Activate(yes);
	}
	
	void OnDestroy() {
		mPoolController = null;
	}
	
	void ChangeState(State state) {
		mCurState = state;
		mCurTime = 0.0f;
		
		switch(state) {
		case State.Inactive:
			if(mPoolController != null) {
				//hm...
				mPoolController = null;
			}
			break;
		case State.Spawn:
			break;
		case State.SpawnWait:
			break;
		}
	}
	
	void Update() {
		switch(mCurState) {
		case State.Inactive:
			break;
		case State.Spawn:
			Transform t = EntityManager.instance.Spawn(type, null, null, null, useSpawnFX);
			mPoolController = t.GetComponentInChildren<PoolDataController>();
			
			Vector3 pos = transform.position;
			
			t.position = new Vector3(pos.x, pos.y, t.position.z);
			
			PlanetAttachStatic pa = t.GetComponentInChildren<PlanetAttachStatic>();
			if(pa != null) {
				pa.RefreshPos();
			}
			
			ChangeState(State.SpawnWait);
			break;
		case State.SpawnWait:
			if(mPoolController != null) {
				if(mPoolController.claimed) {
					if(mCurTime >= delayRespawn) {
						ChangeState(State.Spawn);
					}
					else {
						mCurTime += Time.deltaTime;
					}
				}
			}
			else {
				ChangeState(State.Spawn);
			}
			break;
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawIcon(transform.position, "spawner");
	}
}
