using UnityEngine;
using System.Collections;

public class UpDirToPlayer : MonoBehaviour {
	//make sure to set the initial up dir
	[SerializeField] bool isLockAngle = false;
	[SerializeField] float lockAngle;
	
	private Transform mPlayer = null;
	
	private Vector2 mUp;
	
	void OnDestroy() {
		mPlayer = null;
	}
	
	void Awake() {
		if(isLockAngle) {
			mUp = transform.up;
		}
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
			if(isLockAngle) {
				Vector2 dir = (mPlayer.position - transform.position).normalized;
				Util.Vector2DDirCap(mUp, ref dir, lockAngle);
				transform.rotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;
			}
			else {
				transform.up = (mPlayer.position - transform.position).normalized;
			}
		}
	}
}
