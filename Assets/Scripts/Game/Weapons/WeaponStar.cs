using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponStar : Weapon {
	public string template = "star"; //make sure it's in the entity manager
	
	public Transform[] braids;
	public Transform[] braidAttaches;
	
	private Queue<ItemStar> mStars;
	private ItemStar mCurStar;
	
	protected override void OnInit() {
		mStars = new Queue<ItemStar>(maxAmmo);
	}
	
	protected override void OnEquip() {
		transform.localScale = Vector3.one;
		
		for(int i = 0; i < maxAmmo; i++) {
			Transform parent = i == 0 ? transform : braidAttaches[braidAttaches.Length-i];
			
			Transform t = EntityManager.instance.Spawn(template, "starAmmo", parent, null, false);
			ItemStar star = t.GetComponent<ItemStar>();
			star.glowSprite.gameObject.active = false;
			star.isAmmo = true;
			mStars.Enqueue(star);
		}
		
		PrepAStar();
	}
	
	protected override bool OnCanFire() {
		bool ret = false;
		
		if(mCurStar != null) {
			ret = mCurStar.action == Entity.Action.idle;
		}
		
		return ret;
	}
	
	protected override void OnExpel() {
		if(mCurStar != null) {
			mCurStar.Release();
			mCurStar = null;
		}
		
		//TODO: throw the stars around
		foreach(ItemStar star in mStars) {
			star.Release();
		}
		
		mStars.Clear();
	}
	
	protected override void OnFire() {
		if(mCurStar != null) {
			mCurStar.Throw(grabber, true);
			PrepAStar();
		}
	}
	
	protected override void OnReadyToFire() {
		if(mCurStar != null) {
			mCurStar.WeaponPrep();
		}
	}
	
	void PrepAStar() {
		//mCurStar
		if(mStars.Count > 0) {
			mCurStar = mStars.Dequeue();
			mCurStar.transform.parent = grabber.headAttach;
			mCurStar.transform.localPosition = Vector3.zero;
			mCurStar.transform.localScale = Vector3.one;
			
			mCurStar.glowSprite.gameObject.active = true;
			
			if(isReadyToFire) {
				mCurStar.WeaponPrep();
			}
			else {
				mCurStar.WeaponRefresh();
			}
			
			if(mStars.Count < braids.Length) {
				braids[mStars.Count].gameObject.SetActiveRecursively(false);
			}
		}
		else {
			mCurStar = null;
		}
	}
	
	void OnDestroy() {
		mStars.Clear();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float sx = grabber.headSprite.scale.x;
		if(transform.localScale.x != sx) {
			Vector3 s = Vector3.one;
			s.x = sx;
			transform.localScale = s;
		}
	}
}
