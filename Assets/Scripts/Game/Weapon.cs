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
	
	public bool isReadyToFire {
		get { return mCurFireDelay >= fireDelay; }
	}
	
	private PlayerGrabber mGrabber;
	private int mCurAmmo;
	private float mCurFireDelay;
	private bool mFiring;
	
	//methods used by grabber
	
	//called during initialization
	public void Init(PlayerGrabber grabber) {
		mGrabber = grabber;
		
		OnInit();
	}
	
	public void Equip() {
		mCurFireDelay = fireDelay;
		
		mCurAmmo = maxAmmo;
		
		OnEquip();
						
		StartCoroutine(DoFire());
	}
	
	//throw away
	public void Expel() {
		StopAllCoroutines();
		
		mFiring = false;
		
		OnExpel();
	}
		
	public void Fire(bool fire) {
		if(OnCanFire()) {
			mFiring = fire;
			
			if(fire) {
				if(fireHold) {
					mGrabber.PlayAnimFireHold();
				}
			}
			else {
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
	
	//ready to fire again
	protected virtual void OnReadyToFire() {
	}
	
	void OnDestroy() {
		mGrabber = null;
	}
	
	IEnumerator DoFire() {
		while(true) {
			if(mCurAmmo == 0) {
				if(OnOutOfAmmo()) {
					mGrabber.Expel(); //it'll call our expel stuff
					yield break;
				}
				else { //maybe weapon is refreshing (eg. ammo regen, etc)
					mGrabber.PlayAnimIdle();
				}
			}
			else if(mCurFireDelay < fireDelay) {
				mCurFireDelay += Time.deltaTime;
				
				if(mCurFireDelay >= fireDelay) {
					OnReadyToFire();
				}
			}
			else {
				if(mFiring) {
					mCurFireDelay = 0; //wait again
					
					//fire away
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
			
			yield return new WaitForFixedUpdate();
		}
	}
}
