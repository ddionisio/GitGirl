using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityManager : MonoBehaviour {
	[System.Serializable]
	public class FactoryData {
		public Transform template;
		public float z;
		
		public int startCapacity;
		public int maxCapacity;
		
		public Transform defaultParent;
						
		[System.NonSerialized]
		private List<Transform> available;
		
		private int allocateCounter = 0;
		
		private Transform poolHolder;
		
		private int nameCounter = 0;
		
		public void Init(Transform poolHolder) {
			this.poolHolder = poolHolder;
			
			nameCounter = 0;
			
			available = new List<Transform>(maxCapacity);
			Expand(startCapacity);
		}
		
		public void Expand(int num) {
			for(int i = 0; i < num; i++) {
				//PoolDataController
				Transform t = (Transform)Object.Instantiate(template);
				t.parent = poolHolder;
				
				PoolDataController pdc = t.GetComponent<PoolDataController>();
				if(pdc == null) {
					pdc = t.gameObject.AddComponent<PoolDataController>();
				}
				
				pdc.factoryKey = template.name;
				
				t.gameObject.SetActiveRecursively(false);
				
				available.Add(t);
			}
		}
		
		public void Release(Transform t) {
			t.parent = poolHolder;
			t.gameObject.SetActiveRecursively(false);
			
			available.Add(t);
			allocateCounter--;
		}
		
		public Transform Allocate(string name, Transform parent) {
			if(available.Count == 0) {
				if(allocateCounter+1 > maxCapacity) {
					Debug.LogWarning(template.name+" is expanding beyond max capacity: "+maxCapacity);
					
					Expand(maxCapacity);
				}
				else {
					Expand(1);
				}
			}
			
			Transform t = available[available.Count-1];
			available.RemoveAt(available.Count-1);
			
			t.GetComponent<PoolDataController>().claimed = false;
			
			t.name = string.IsNullOrEmpty(name) ? template.name + (nameCounter++) : name;
			t.parent = parent == null ? defaultParent : parent;
			t.localPosition = new Vector3(0.0f, 0.0f, z);
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			
			allocateCounter++;
			return t;
		}
		
		public void DeInit() {
			available.Clear();
			
			poolHolder = null;
					
			allocateCounter = 0;
		}
	}
	
	[SerializeField]
	FactoryData[] factory;
	
	[SerializeField]
	Transform poolHolder;
	
	[SerializeField]
	Transform spawnTemplate;
	
	[SerializeField]
	Transform spawnEffectGroup;
	
	[SerializeField]
	int spawnEffectMax;
	
	private static EntityManager mInstance = null;
	
	private Dictionary<string, FactoryData> mFactory;
	
	private SpawnEffect[] mSpawnEffects;
	private int mCurSpawnInd;
	
	public static EntityManager instance {
		get {
			return mInstance;
		}
	}
	
	//if toParent is null, then set parent to us or factory's default
	public Transform Spawn(string type, string name, Transform toParent, string waypoint, bool useFX=false) {
		Transform ret = null;
		
		FactoryData dat;
		if(mFactory.TryGetValue(type, out dat)) {
			ret = dat.Allocate(name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);
			
			if(ret != null) {				
				if(!string.IsNullOrEmpty(waypoint)) {
					Transform wp = WaypointManager.instance.GetWaypoint(waypoint);
					if(wp != null) {
						ret.position = wp.position;
					}
				}
				
				ret.gameObject.SetActiveRecursively(true);
				
				Entity entity = ret.GetComponentInChildren<Entity>();
				if(entity != null) {
					entity.Spawn();
					
					if(useFX) {
						StartCoroutine(ApplySpawn(entity));
					}
				}
			}
		}
		else {
			Debug.LogWarning("No such type: "+type+" attempt to allocate: "+name);
		}
		
		return ret;
	}
	
	IEnumerator ApplySpawn(Entity e) {
		yield return new WaitForFixedUpdate();
		
		//get available spawn fx
		if(mCurSpawnInd == mSpawnEffects.Length) {
			for(int i = 0; i < mSpawnEffects.Length; i++) {
				if(!mSpawnEffects[i].gameObject.active) {
					mCurSpawnInd = i;
					break;
				}
			}
			
			if(mCurSpawnInd == mSpawnEffects.Length) {
				Debug.LogWarning("No more spawn effects...");
				yield break;
			}
		}
		
		SpawnEffect fx = mSpawnEffects[mCurSpawnInd];
		mCurSpawnInd++;
		
		Vector3 entPos = e.transform.position;
		fx.transform.position = new Vector3(entPos.x, entPos.y, fx.transform.position.z);
		
		fx.gameObject.SetActiveRecursively(true);
		
		//base scale on radius
		if(e.planetAttach == null) {
			fx.Begin(1.0f, OnSpawnEnd);
		}
		else {
			Bounds b = fx.sprite.GetBounds();
			float s = (e.planetAttach.radius*2.0f + fx.sizeOffset)/(b.size.y > b.size.x ? b.size.y : b.size.x);
			fx.Begin(s, OnSpawnEnd);
		}
		
		yield break;
	}
	
	public void Release(Transform t) {
		PoolDataController pdc = t.GetComponent<PoolDataController>();
		if(pdc != null) {
			FactoryData dat;
			if(mFactory.TryGetValue(pdc.factoryKey, out dat)) {
				pdc.claimed = true;
				dat.Release(t);
			}
		}
		else { //not in the pool, just kill it
			Object.Destroy(t.gameObject);
		}
	}
	
	void OnDestroy() {
		mInstance = null;
		
		foreach(FactoryData dat in mFactory.Values) {
			dat.DeInit();
		}
	}
			
	void Awake() {
		mInstance = this;
		
		//generate cache and such
		mFactory = new Dictionary<string, FactoryData>(factory.Length);
		foreach(FactoryData factoryData in factory) {
			factoryData.Init(poolHolder);
			
			mFactory.Add(factoryData.template.name, factoryData);
		}
		
		poolHolder.gameObject.SetActiveRecursively(false);
		
		//spawn fxs
		mCurSpawnInd = 0;
		mSpawnEffects = new SpawnEffect[spawnEffectMax];
		for(int i = 0; i < spawnEffectMax; i++) {
			Transform newSpawnFX = (Transform)Object.Instantiate(spawnTemplate);
			
			Transform t = newSpawnFX.transform;
			t.parent = spawnEffectGroup;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			mSpawnEffects[i] = t.GetComponent<SpawnEffect>();
			t.gameObject.SetActiveRecursively(false);
		}
	}
	
	private void OnSpawnEnd(SpawnEffect fx) {
		fx.gameObject.SetActiveRecursively(false);
	}
}
