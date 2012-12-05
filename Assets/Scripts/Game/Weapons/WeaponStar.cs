using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponStar : Weapon {
	public string template = "star"; //make sure it's in the entity manager
	
	private Queue<ItemStar> mStars;
	private ItemStar mCurStar;
	
	protected override void OnInit() {
		mStars = new Queue<ItemStar>(maxAmmo);
	}
	
	protected override void OnEquip() {
		for(int i = 0; i < maxAmmo; i++) {
			Transform t = EntityManager.instance.Spawn(template, "starAmmo", transform, null, false);
			ItemStar star = t.GetComponent<ItemStar>();
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
	
	void PrepAStar() {
		//mCurStar
		if(mStars.Count > 0) {
			mCurStar = mStars.Dequeue();
			mCurStar.transform.parent = grabber.headAttach;
			mCurStar.transform.localPosition = Vector3.zero;
			mCurStar.WeaponPrep();
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
	
	}
}
