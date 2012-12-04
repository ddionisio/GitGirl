using UnityEngine;
using System.Collections;

public class UpDirToPlayer : MonoBehaviour {
	
	private Transform mPlayer = null;
	
	void OnDestroy() {
		mPlayer = null;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(mPlayer == null) {
			SceneLevel sl = SceneLevel.instance;
			if(sl != null && sl.player != null) {
				mPlayer = sl.player.transform;
			}
		}
		else {
			transform.up = (mPlayer.position - transform.position).normalized;
		}
	}
}
