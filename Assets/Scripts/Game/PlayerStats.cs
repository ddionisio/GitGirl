using UnityEngine;
using System.Collections;

public class PlayerStats : EntityStats {
	
	private int mScore = 0;
	
	private int mCurComboScore = 0;
	private int mCurCombo = 0;
	
	public int score {
		get {
			return mScore+(mCurComboScore*mCurCombo);
		}
		
		set {
			mScore = value;
		}
	}
	
	public override void ResetStats () {
		base.ResetStats ();
	}
	
	public void AddScore(int amt) {
		//refresh score
		//check combo
		UIManager.instance.hud.combo.Refresh(amt);
		
		mCurComboScore = UIManager.instance.hud.combo.curPoints;
		mCurCombo = UIManager.instance.hud.combo.curCombo;
	}
	
	void Start() {
		UIManager.instance.hud.combo.finishCallback = OnComboFinish;
	}
	
	void OnComboFinish(HUDCombo combo) {
		mScore += mCurCombo*mCurComboScore;
		
		mCurCombo = mCurComboScore = 0;
		
		//update score
		UIManager.instance.hud.score.score = mScore;
	}
}
