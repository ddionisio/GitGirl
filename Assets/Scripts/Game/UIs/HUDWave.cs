using UnityEngine;
using System.Collections;

public class HUDWave : MonoBehaviour {
	[SerializeField] UILabel label;
	[SerializeField] string waveFormat;
	
	private int mCurWave;
	
	public int curWave {
		get {
			return mCurWave;
		}
	}
	
	public void SetWave(int cur, int max) {
		mCurWave = cur;
		
		if(max == 0) {
			label.enabled = false;
		}
		else {
			label.enabled = true;
			label.text = string.Format(waveFormat, cur, max);
		}
	}
	
	void Awake() {
		label.enabled = false;
	}
}
