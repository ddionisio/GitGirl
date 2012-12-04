using UnityEngine;
using System.Collections;

public class HUDScore : MonoBehaviour {
	[SerializeField] UILabel label;
	[SerializeField] string scoreFormat;
	[SerializeField] float delay = 0.5f;
	
	private int mCurNumber;
	private int mPrevNumber;
	private int mNextNumber;
	
	private float mCurDelay;
	
	public int score {
		get {
			return mNextNumber;
		}
		set {
			mPrevNumber = mCurNumber;
			mNextNumber = value;
			mCurDelay = 0.0f;
		}
	}
	
	public void Clear() {
		mCurNumber = mPrevNumber = mNextNumber;
		mCurDelay = 0.0f;
	}
	
	//animate numbers as it changes
	void Update() {
		if(mCurNumber != mNextNumber) {
			
			mCurDelay += Time.deltaTime;
			if(mCurDelay >= delay) {
				mCurNumber = mNextNumber;
			}
			else {
				mCurNumber = Mathf.RoundToInt(Mathf.Lerp((float)mPrevNumber, (float)mNextNumber, mCurDelay/delay));
			}
			
			label.text = string.Format(scoreFormat, mCurNumber);
		}
	}
}
