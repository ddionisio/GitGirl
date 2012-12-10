using UnityEngine;
using System.Collections;

//attach these to sprites you want to be affected by entity
public class SpriteEntityController : MonoBehaviour, Entity.IListener {
	public float invulBlinkDelay = 0.05f;
	
	protected tk2dBaseSprite mSprite;
	protected tk2dAnimatedSprite mSpriteAnim;
	
	private Color mPrevColor;
	private int[] mActionAnimIds;
	
	private bool mInvulDoBlink = false;
	private float mInvulCurTime = 0.0f;
	
	protected virtual void Awake() {
		mSprite = GetComponent<tk2dBaseSprite>();
		mSpriteAnim = mSprite as tk2dAnimatedSprite;
	}
	
	protected virtual void Start() {
	}
	
	protected virtual void Update() {
		if(mInvulDoBlink) {
			mInvulCurTime += Time.deltaTime;
			if(mInvulCurTime >= invulBlinkDelay) {
				mInvulCurTime = 0.0f;
				
				Color c = mSprite.color;
				c.a = c.a == 0.0f ? mPrevColor.a : 0.0f;
				mSprite.color = c;
			}
		}
	}
	
	protected bool HasAnim(Entity.Action act) {
		return mActionAnimIds[(int)act] != -1;
	}
	
	protected void PlayAnim(Entity.Action act) {
		if(mSpriteAnim != null) {
			if(mActionAnimIds == null) {
				mActionAnimIds = new int[(int)Entity.Action.NumActions];
				for(int i = 0; i < mActionAnimIds.Length; i++) {
					mActionAnimIds[i] = mSpriteAnim.GetClipIdByName(((Entity.Action)i).ToString());
				}
			}
			
			int id = mActionAnimIds[(int)act];
			if(id != -1 && mSpriteAnim.clipId != id) {
				mSpriteAnim.Play(id);
			}
		}
	}

	public virtual void OnEntityAct(Entity.Action act) {
		PlayAnim(act);
	}
	
	public virtual void OnEntityInvulnerable(bool yes) {
		mInvulDoBlink = yes;
		if(yes) {
			mPrevColor = mSprite.color;
			mInvulCurTime = 0.0f;
		}
		else {
			mSprite.color = mPrevColor;
		}
	}
	
	public virtual void OnEntityCollide(Entity other, RaycastHit hit, bool youAreReceiver) {
	}
	
	public virtual void OnEntitySpawnFinish() {
	}
}
