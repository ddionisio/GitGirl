using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReticleManager : MonoBehaviour {
	public const float childZOfs = -2.0f;
	
	//all inactive reticles are children
	public Transform template;
	
	private struct ReticleHolder {
		public EntityBase targettedEntity;
		public Reticle reticle;
		
		public ReticleHolder(EntityBase target, Reticle r) {
			targettedEntity = target;
			reticle = r;
		}
	}
	
	private List<ReticleHolder> mActiveReticles = new List<ReticleHolder>();
	
	/// <summary>
	/// Get a target if there's one. First element in list.
	/// </summary>
	public Transform GetTarget() {
		Transform t = null;
		if(mActiveReticles.Count > 0) {
			t = mActiveReticles[0].reticle.transform.parent;
		}
		
		return t;
	}
	
	public int GetNumActive() {
		return mActiveReticles.Count;
	}
	
	//typeOverride = use to discard what reticle entity is set to,
	// or if there's no entity
	public Reticle Activate(Transform attachTo, Reticle.Type typeOverride=Reticle.Type.NumType) {
		Reticle ret = null;
		EntityBase ent = attachTo.GetComponentInChildren<EntityBase>();
		
		//verify reticle type
		Reticle.Type reticleType = typeOverride;
		if(ent != null && reticleType == Reticle.Type.NumType) {
			reticleType = ent.reticle;
		}
		
		if(reticleType != Reticle.Type.NumType) {
			ret = attachTo.GetComponentInChildren<Reticle>();
			
			//add a reticle to target
			if(ret == null) {
				Transform trans = transform;
				Transform child = null;
				
				//get or create a reticle
				if(trans.childCount > 0) {
					child = trans.GetChild(0);
					child.gameObject.SetActiveRecursively(true);
				}
				else {
					child = Transform.Instantiate(template) as Transform;
				}
										
				//add child to attachee, then switch its layer to target for grabbing
				child.parent = attachTo;
				Transform childTrans = child.transform;
				Vector2 parentPos = child.parent.position;
				childTrans.position = new Vector3(parentPos.x, parentPos.y, childZOfs);
				childTrans.localRotation = Quaternion.identity;
				childTrans.localScale = Vector3.one;
				
				ret = childTrans.GetComponentInChildren<Reticle>();
				
				mActiveReticles.Add(new ReticleHolder(ent, ret));
			}
			
			//set data
			if(ent != null && !ent.FlagsCheck(Entity.Flag.Targetted)) {
				ent.FlagsAdd(Entity.Flag.Targetted);
				ent.OnTargetted(true);
			}
									
			ret.Activate(reticleType, ent);
		}
		
		return ret;
	}
	
	public void ActivateInRange(Vector3 pos, float radius, int layerMask, Reticle.Type typeOverride=Reticle.Type.NumType) {
		//TODO: optimize
		DeactivateAll();
		
		RaycastHit[] hits = Physics.SphereCastAll(pos, radius, new Vector3(0,0,1.0f), Mathf.Infinity, layerMask);
		if(hits.Length > 0) {
			foreach(RaycastHit hit in hits) {
				Transform t = hit.transform;
				Activate(t, typeOverride);
			}
		}
	}
	
	//deactivate a reticle residing in a target
	public void DeactivateFromTarget(Transform target) {
		//look for it
		Reticle r = target.GetComponentInChildren<Reticle>();
		if(r != null) {
			for(int i = 0; i < mActiveReticles.Count; i++) {
				ReticleHolder rh = mActiveReticles[i];
				EntityBase e = rh.targettedEntity;
				
				if(rh.reticle == r) {
					//cache the reticle
					r.transform.parent = transform;
					r.gameObject.SetActiveRecursively(false);
					
					if(e != null) {
						e.FlagsRemove(Entity.Flag.Targetted);
						e.OnTargetted(false);
					}
					
					mActiveReticles.RemoveAt(i);
					break;
				}
			}
		}
	}
	
	public void DeactivateAll(Transform exclude = null) {
		Reticle reticleExclude = null;
		EntityBase reticleExcludeEnt = null;
		
		foreach(ReticleHolder rh in mActiveReticles) {
			Reticle r = rh.reticle;
			EntityBase e = rh.targettedEntity;
			
			if(r.transform.parent != exclude) {
				//cache the reticle
				r.transform.parent = transform;
				r.gameObject.SetActiveRecursively(false);
				
				if(e != null) {
					e.FlagsRemove(Entity.Flag.Targetted);
					e.OnTargetted(false);
				}
			}
			else if(reticleExclude == null) {
				reticleExclude = r;
				reticleExcludeEnt = rh.targettedEntity;
			}
		}
		
		mActiveReticles.Clear();
		
		//re-add excluded reticle
		if(reticleExclude != null) {
			mActiveReticles.Add(new ReticleHolder(reticleExcludeEnt, reticleExclude));
		}
	}
	
	public void Clear() {
		foreach(ReticleHolder rh in mActiveReticles) {
			Object.Destroy(rh.reticle.gameObject);
		}
		
		foreach(Transform c in transform) {
			Object.Destroy(c.gameObject);
		}
		
		mActiveReticles.Clear();
	}
	
	void SceneShutdown() {
		Clear();
	}
	
	void OnDestroy() {
		//remove references
		mActiveReticles.Clear();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
