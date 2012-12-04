using UnityEngine;
using System.Collections;

public class SpawnEffect : MonoBehaviour {
	public delegate void OnSpawnEnd(SpawnEffect fx);
	
	public tk2dBaseSprite sprite;
	
	public float sizeOffset;
	
	public float startDelay;
	public float waitDelay;
	public float endDelay;
	
	public float rotatePerSecond;
	
	private enum State {
		Start,
		Wait,
		End,
		Ended
	}
	
	private State mState;
	
	private float mCurTime;
	
	private float mSpawnScale;
	
	private OnSpawnEnd mSpawnEndCallback;
	
	public bool hasEnded {
		get {
			return mState == State.Ended;
		}
	}
	
	public void Begin(float spawnerEndScale, OnSpawnEnd callback) {
		mSpawnEndCallback = callback;
		
		mSpawnScale = spawnerEndScale;
		
		SetState(State.Start);
	}
	
	void OnDestroy() {
		mSpawnEndCallback = null;
	}

	void Awake () {
	}
	
	void ApplyT(float t) {
		Color c = sprite.color;
		c.a = t;
		sprite.color = c;
		
		Vector2 s = Vector2.one*t*mSpawnScale;
		sprite.scale = new Vector3(s.x, s.y, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		mCurTime += Time.deltaTime;
		
		switch(mState) {
		case State.Start:
			if(mCurTime >= startDelay) {
				ApplyT(1.0f);
				SetState(State.Wait);
			}
			else {
				ApplyT(Ease.In(mCurTime, startDelay, 0.0f, 1.0f));
			}
			break;
		case State.Wait:
			if(mCurTime >= waitDelay) {
				SetState(State.End);
			}
			break;
		case State.End:
			if(mCurTime >= endDelay) {
				ApplyT(0.0f);
				SetState(State.Ended);
			}
			else {
				ApplyT(Ease.Out(mCurTime, endDelay, 1.0f, -1.0f));
			}
			break;
		}
		
		Vector3 ea = transform.eulerAngles;
		ea.z += Time.deltaTime*rotatePerSecond;
		ea.z %= 360.0f;
		transform.eulerAngles = ea;
	}
	
	private void SetState(State s) {
		mCurTime = 0;
		mState = s;
		
		switch(mState) {
		case State.Ended:
			sprite.scale = Vector3.one;
			sprite.color = Color.white;
			
			if(mSpawnEndCallback != null) {
				mSpawnEndCallback(this);
			}
			break;
		}
	}
}
