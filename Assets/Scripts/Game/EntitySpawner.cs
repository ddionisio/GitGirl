using UnityEngine;
using System.Collections;

public class EntitySpawner : MonoBehaviour {
	//continuously spawn crap, use for projectiles
	
	public string type;
	
	//these are only if there's a planet attacher
	public bool useUpVector = false;
	
	public float angleMin; //starting from Vector2.up
	public float angleMax;
	
	public float speedMin;
	public float speedMax;
	public float radiusSpread;
	
	public bool horizontalOnly;
	
	public int countPerPeriod;
	
	public float delayPerPeriod;
		
	public float delayPeriod;
	
	public int countPeriod = -1; //-1 for infinite
	
	public bool useSpawnFX = false;
	
	public bool activeOnStart;
	
	enum State {
		Inactive,
		Spawn,
		SpawnWait,
		PeriodWait
	}
	
	private State mCurState = State.Inactive;
	private float mCurTime = 0;
	private int mCurSpawn;
	private int mCurPeriod;
	
	public void Activate(bool yes) {
		if(yes) {
			mCurPeriod = 0;
			mCurSpawn = 0;
			ChangeState(State.SpawnWait);
		}
		else {
			ChangeState(State.Inactive);
		}
	}
	
	void Start() {
		if(activeOnStart) {
			Activate(true);
		}
	}
	
	void OnSceneActivate(bool yes) {
		Activate(yes);
	}
	
	void ChangeState(State state) {
		mCurState = state;
		mCurTime = 0.0f;
		
		switch(state) {
		case State.Inactive:
			mCurPeriod = 0;
			break;
		case State.Spawn:
			mCurSpawn++;
			if(mCurSpawn > countPerPeriod) {
				ChangeState(State.PeriodWait);
			}
			break;
		case State.SpawnWait:
			break;
		case State.PeriodWait:
			mCurSpawn = 0;
			mCurPeriod++;
			if(countPeriod > 0 && mCurPeriod > countPeriod) {
				ChangeState(State.Inactive);
			}
			break;
		}
	}
	
	void Update() {
		switch(mCurState) {
		case State.Inactive:
			break;
		case State.Spawn:
			Transform t = EntityManager.instance.Spawn(type, null, null, null, useSpawnFX);
			
			//set position
			Vector3 spawnPos = t.position;
			Vector3 pos = transform.position;
			Vector2 posSpread = Random.insideUnitCircle*radiusSpread;
			pos.x += posSpread.x;
			pos.y += posSpread.y;
			pos.z = spawnPos.z;
			t.position = pos;
			
			float angle = angleMin != angleMax ? Random.Range(angleMin, angleMax)*Mathf.Deg2Rad : angleMin;
			float speed = speedMin != speedMax ? Random.Range(speedMin, speedMax) : speedMin;
			
			ProjBullet proj = t.GetComponentInChildren<ProjBullet>();
			if(proj != null) {
				if(useUpVector) {
					Vector2 u = transform.up;
					proj.dir = u;
				}
				else {
					proj.dir = Vector2.up;
				}
				
				if(angle != 0) {
					proj.dir = Util.Vector2DRot(proj.dir, angle);
				}
				
				proj.speed = speed;
			}
			else {
				PlanetAttach pa = t.GetComponentInChildren<PlanetAttach>();
				if(pa != null) {
					Vector2 dir;
					
					if(useUpVector) {
						Vector2 u = transform.up;
						dir = pa.ConvertToPlanetDir(angle != 0 ? Util.Vector2DRot(u, angle) : u);
					}
					else {
						dir = angle != 0 ? Util.Vector2DRot(Vector2.up, angle) : Vector2.up;
					}
					
					if(horizontalOnly) {
						dir.x = Mathf.Sign(dir.x);
						dir.y = 0;
					}
					
					pa.velocity = dir*speed;
					pa.RefreshPos();
					pa.ResetCurYVel();
				}
			}
			
			ChangeState(State.SpawnWait);
			break;
		case State.SpawnWait:
			mCurTime += Time.deltaTime;
			if(mCurTime >= delayPerPeriod) {
				ChangeState(State.Spawn);
			}
			break;
		case State.PeriodWait:
			mCurTime += Time.deltaTime;
			if(mCurTime >= delayPeriod) {
				ChangeState(State.Spawn);
			}
			break;
		}
	}
}
