using UnityEngine;
using System.Collections;

public class HUDCombo : MonoBehaviour {
	public delegate void OnFinish(HUDCombo combo);
	
	[SerializeField] UILabel pointsLabel;
	
	[SerializeField] string pointsFormat;
	
	[SerializeField] Animation popAnim;
	
	[SerializeField] float fadeDelayMax;
	[SerializeField] float fadeDelayMin;
	[SerializeField] float fadeDelayDiminish;
	
	[SerializeField] int diminishAtCombo;
	
	public OnFinish finishCallback = null;
	
	private bool mDoFade = false;
	private int mCurPoints = 0;
	private int mCurCombo = 0;
	
	private float mFadeCurMaxTime;
	private float mFadeCurTime;
	
	public int curCombo {
		get {
			return mCurCombo;
		}
	}
	
	public int curPoints {
		get {
			return mCurPoints;
		}
	}
	
	void Finish() {
		if(finishCallback != null) {
			finishCallback(this);
		}
		
		popAnim.Stop();
		
		mDoFade = false;
		mCurCombo = 0;
		mCurPoints = 0;
		
		gameObject.SetActiveRecursively(false);
	}
	
	public void Refresh(int points) {
		gameObject.SetActiveRecursively(true);
		
		popAnim.Play();
		
		mCurPoints += points;
		
		mCurCombo++;
		
		mDoFade = true;
		mFadeCurTime = 0;
		
		if(mCurCombo >= diminishAtCombo) {
			float d = (float)(mCurCombo-diminishAtCombo);
			mFadeCurMaxTime = fadeDelayMax - d*fadeDelayDiminish;
			if(mFadeCurMaxTime < fadeDelayMin) {
				mFadeCurMaxTime = fadeDelayMin;
			}
		}
		else {
			mFadeCurMaxTime = fadeDelayMax;
		}
		
		pointsLabel.text = string.Format(pointsFormat, mCurPoints, mCurCombo);
		
		pointsLabel.color = Color.white;
	}
	
	void SceneShutdown() {
		mDoFade = false;
		mCurCombo = 0;
		mCurPoints = 0;
		finishCallback = null;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(mDoFade) {
			mFadeCurTime += Time.deltaTime;
			if(mFadeCurTime >= mFadeCurMaxTime) {
				Finish();
			}
			else {
				Color c = Color.white;
				c.a = 1.0f - mFadeCurTime/mFadeCurMaxTime;
				pointsLabel.color = c;
			}
		}
	}
}
