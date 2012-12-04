using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDEntityStatus : MonoBehaviour {
	public Transform hitpointTemplate;
	
	public Transform container; //this is where we put the hearts
	public Transform cacheContainer; //this is where we put cache
	
	public NGUILayoutAnchor containerFrameLayout; //the frame around the hearts
	public NGUILayoutFlow containerLayout; //the one that holds the hearts
	
	public UILabel nameWidget;
	public UISprite portraitWidget;
	public UISprite hpFrame;
	
	public float hurtPanelFeedbackDelay;
	public float hurtPanelBlinkDelay;
	
	public float lastHPBlinkDelay; //set to 0 to disable
	
	public bool isHPFlipped = false;
	
	private List<HUDHitPoint> mHPs = new List<HUDHitPoint>();
	private EntityStats mStats;
	private int mCurHP;
	
	private float mCurPanelFeedbackDelay=0;
	private float mCurPanelBlinkDelay=0;
	private float mCurHPBlinkDelay=0;
	
	public void SetStats(EntityStats stats) {
		mStats = stats;
		
		if(stats == null) {
			Clear();
		}
		else if(gameObject.active) { //refresh later
			RefreshStats(true);
		}
	}
	
	public void RefreshStats(bool refreshHP) {
		if(refreshHP) {
			if(mStats.maxHP != mHPs.Count) {
				Clear();
				
				for(int i = 0; i < mStats.maxHP; i++) {
					Transform hpTrans;
					if(cacheContainer.GetChildCount() > 0) {
						hpTrans = cacheContainer.GetChild(0);
					}
					else {
						hpTrans = (Transform)Object.Instantiate(hitpointTemplate);
					}
					
					hpTrans.parent = container;
					hpTrans.localPosition = Vector3.zero;
					hpTrans.localRotation = Quaternion.identity;
					hpTrans.localScale = Vector3.one;
					hpTrans.gameObject.SetActiveRecursively(true);
					
					mHPs.Add(hpTrans.GetComponent<HUDHitPoint>());
				}
				
				if(containerLayout != null) {
					containerLayout.Reposition();
				}
				
				mHPs.Sort(delegate(HUDHitPoint h1, HUDHitPoint h2) {
					Vector3 p1 = h1.transform.localPosition;
					Vector3 p2 = h2.transform.localPosition;
					return isHPFlipped ? Mathf.RoundToInt(p2.x-p1.x) : Mathf.RoundToInt(p1.x-p2.x);
				});
				
				if(containerFrameLayout != null) {
					containerFrameLayout.Reposition();
				}
			}
			
			int curHPInd = 0;
			for(; curHPInd < mStats.curHP; curHPInd++) {
				mHPs[curHPInd].SetOn(true);
			}
			
			for(; curHPInd < mStats.maxHP; curHPInd++) {
				mHPs[curHPInd].SetOn(false);
			}
			
			mCurHP = mStats.curHP;
		}
		
		mCurPanelFeedbackDelay = hurtPanelFeedbackDelay;
		
		if(nameWidget != null) {
			nameWidget.text = mStats.displayName;
		}
		
		if(portraitWidget != null) {
			if(!string.IsNullOrEmpty(mStats.portrait)) {
				portraitWidget.gameObject.SetActiveRecursively(true);
				portraitWidget.spriteName = mStats.portrait;
				portraitWidget.MakePixelPerfect();
			}
			else {
				portraitWidget.gameObject.SetActiveRecursively(false);
			}
		}
	}
	
	void OnEnable() {
		cacheContainer.gameObject.SetActiveRecursively(false);
		if(mStats != null) {
			RefreshStats(true);
		}
		else {
			Clear();
		}
	}
	
	void OnDisable() {
	}
	
	void Awake() {
		cacheContainer.gameObject.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(mStats != null && mStats.curHP != mCurHP) {
			if(mCurHP > mStats.curHP) { //decrease
				for(int i = mCurHP-1; i >= mStats.curHP; i--) {
					mHPs[i].SetOn(false);
				}
				
				mCurPanelBlinkDelay = mCurPanelFeedbackDelay = 0;
			}
			else { //increase
				if(mCurHP > 0) {
					mHPs[mCurHP-1].onSprite.color = Color.white; //in case we increase from 1 while it's blinked out
				}
				
				for(int i = mCurHP; i < mStats.curHP; i++) {
					mHPs[i].SetOn(true);
				}
			}
			
			mCurHP = mStats.curHP;
		}
		
		//hp panel feedback
		if(mCurPanelFeedbackDelay < hurtPanelFeedbackDelay) {
			mCurPanelFeedbackDelay += Time.deltaTime;
			if(mCurPanelFeedbackDelay >= hurtPanelFeedbackDelay) {
				hpFrame.color = Color.white;
			}
			else {
				mCurPanelBlinkDelay += Time.deltaTime;
				if(mCurPanelBlinkDelay >= hurtPanelBlinkDelay) {
					Color c = hpFrame.color;
					c.a = c.a == 1.0f ? 0.0f : 1.0f;
					hpFrame.color = c;
					mCurPanelBlinkDelay = 0;
				}
			}
		}
		
		//last hp feedback
		if(mCurHP == 1 && lastHPBlinkDelay > 0) {
			mCurHPBlinkDelay += Time.deltaTime;
			if(mCurHPBlinkDelay >= lastHPBlinkDelay) {
				Color c = mHPs[mCurHP-1].onSprite.color;
				c.a = c.a == 1.0f ? 0.0f : 1.0f;
				mHPs[mCurHP-1].onSprite.color = c;
				mCurHPBlinkDelay = 0;
			}
		}
	}
	
	void Clear() {
		foreach(HUDHitPoint hp in mHPs) {
			hp.transform.parent = cacheContainer;
			hp.gameObject.SetActiveRecursively(false);
		}
		
		mHPs.Clear();
	}
}
