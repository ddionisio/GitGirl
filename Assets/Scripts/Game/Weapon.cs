using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	public enum Type {
		Star,
		
		NumTypes
	}
	
	public Type type;
	public int maxAmmo;
	public int ammoConsume = 1;
	public float fireDelay;
	public bool fireHold;
	
	public int curAmmo {
		get { return mCurAmmo; }
	}
	
	public PlayerGrabber grabber {
		get { return mGrabber; }
	}
	
	private PlayerGrabber mGrabber;
	private int mCurAmmo;
	
	//methods used by grabber
	
	//called during initialization
	public void Init(PlayerGrabber grabber) {
		mGrabber = grabber;
		
		OnInit();
	}
	
	public void Equip() {
		mCurAmmo = maxAmmo;
		
		OnEquip();
	}
	
	//throw away
	public void Expel() {
		CancelInvoke();
		
		OnExpel();
	}
		
	public void Fire(bool fire) {
		if(OnCanFire()) {
			if(fire) {
				if(fireHold) {
					mGrabber.PlayAnimFireHold();
				}
				
				InvokeRepeating("DoFire", 0, fireDelay);
			}
			else {
				CancelInvoke();
				
				mGrabber.PlayAnimIdle();
			}
		}
	}
	
	//make sure to only initialize containers, etc. don't grab stuff yet from managers
	protected virtual void OnInit() {
	}
	
	protected virtual void OnEquip() {
	}
	
	protected virtual bool OnCanFire() {
		return true;
	}
	
	protected virtual void OnExpel() {
	}
	
	protected virtual void OnFire() {
	}
	
	//return true to expel
	protected virtual bool OnOutOfAmmo() {
		return true;
	}
	
	void OnDestroy() {
		mGrabber = null;
	}
	
	void DoFire() {
		if(mCurAmmo == 0) {
			CancelInvoke();
			
			if(OnOutOfAmmo()) {
				mGrabber.Expel(); //it'll call our expel stuff
			}
			else { //maybe weapon is refreshing (eg. ammo regen, etc)
				mGrabber.PlayAnimIdle();
			}
		}
		else if(OnCanFire()) {
			mCurAmmo-=ammoConsume;
			if(mCurAmmo < 0) {
				mCurAmmo = 0;
			}
			
			if(!fireHold) {
				mGrabber.PlayAnimThrow();
			}
			
			OnFire();
		}
	}
}
