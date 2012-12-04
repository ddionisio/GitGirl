using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour {
	public const string levelStateKey = "lst";
	public const string levelScoreKey = "lscr";
	
	public enum LevelState {
		Locked,
		Unlocked,
		Complete
	}
	
	public LevelState GetLevelState(int level) {
		LevelState ret = LevelState.Locked;
		
		string key = levelStateKey+level;
		if(level == 0 && !PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetInt(key, (int)LevelState.Unlocked);
			ret = LevelState.Unlocked;
		}
		else {
			ret = (LevelState)PlayerPrefs.GetInt(levelStateKey+level, (int)LevelState.Locked);
		}
		
		return ret;
	}
	
	public int GetLevelScore(int level) {
		return PlayerPrefs.GetInt(levelScoreKey+level, 0);
	}
		
	public void SetLevelState(int level, LevelState state) {
		PlayerPrefs.SetInt(levelStateKey+level, (int)state);
	}
	
	public void SetCurrentLevelState(LevelState state) {
		int curLevel = Main.instance.sceneManager.curLevel;
		SetLevelState(curLevel, state);
		
		//unlock next level
		if(state == LevelState.Complete && GetLevelState(curLevel+1) == LevelState.Locked) {
			SetLevelState(curLevel+1, LevelState.Unlocked);
		}
	}
	
	public void SetLevelScore(int level, int score) {
		PlayerPrefs.SetInt(levelScoreKey+level, score);
	}
	
	public void SetCurrentLevelScore(int score) {
		int curLevel = Main.instance.sceneManager.curLevel;
		
		//make sure score is higher
		if(GetLevelScore(curLevel) < score) {
			SetLevelScore(curLevel, score);
		}
	}
		
	void Awake() {
	}
}
