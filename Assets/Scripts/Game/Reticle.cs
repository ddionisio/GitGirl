using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	public enum Type {
		Grab,
		Hit,
		Eat,
		
		NumType
	}
	
	private static string[] mTypeClips = {"grab", "hit", "eat"};
	
	public tk2dAnimatedSprite animSprite;
	
	public float minScale;
	public float minAlpha;
	public float activateDelay;
	
	public float rotatePerSecond;
	
	private float mCurDelay;
	
	private Vector3 mScale = Vector3.one;
	private Color mColor = Color.white;
	
	private enum State {
		None,
		Activate,
		Active
	};
	
	private State mState = State.None;
	
	private EntityBase mEnt;
	
	public EntityBase entity {
		get {
			return mEnt;
		}
	}
	
	public void Activate(Type type, EntityBase ent) {
		animSprite.Play(mTypeClips[(int)type]);
		mEnt = ent;
		mState = State.Activate;
		mCurDelay = 0;
	}
	
	void OnDisable() {
		mEnt = null;
	}
	
	void Awake() {
		if(animSprite == null) {
			animSprite = GetComponentInChildren<tk2dAnimatedSprite>();
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		switch(mState) {
		case State.Activate:
			mCurDelay += Time.deltaTime;
			if(mCurDelay >= activateDelay) {
				animSprite.color = Color.white;
				animSprite.scale = Vector3.one;
				
				mCurDelay = 0.0f;
				mState = State.Active;
			}
			else {
				mColor.a = Ease.In(mCurDelay, activateDelay, minAlpha, 1.0f-minAlpha);
				mScale.x = mScale.y = Ease.In(mCurDelay, activateDelay, minScale, 1.0f-minScale);
				
				animSprite.color = mColor;
				animSprite.scale = mScale;
			}
			break;
			
		case State.Active:
			break;
		}
		
		Vector3 r = transform.localEulerAngles;
		r.z += rotatePerSecond*Time.deltaTime;
		transform.localEulerAngles = r;
	}
}
