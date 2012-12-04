using UnityEngine;
using System.Collections;

public class EntityStats : MonoBehaviour {
	public string displayName; //name to display in hud
	public string portrait;
	
	public int maxHP = 1;
	
	public int damage = 1;
	
	private int mCurHP;
	
	public int curHP {
		get {
			return mCurHP;
		}
	}
	
	public bool isFullHealth {
		get {
			return mCurHP == maxHP;
		}
	}
	
	public void ApplyDamage(EntityStats src) {
		if(src != null) {
			ApplyDamage(src.damage);
		}
	}
	
	public void ApplyDamage(int amt) {
		mCurHP -= amt;
		if(mCurHP < 0) {
			mCurHP = 0;
		}
		else if(mCurHP > maxHP) {
			mCurHP = maxHP;
		}
	}
	
	public virtual void ResetStats() {
		mCurHP = maxHP;
	}
	
	void Awake() {
		ResetStats();
	}
}
