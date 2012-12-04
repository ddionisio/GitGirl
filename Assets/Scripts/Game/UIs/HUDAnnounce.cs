using UnityEngine;
using System.Collections;

public class HUDAnnounce : MonoBehaviour {
	public enum State {
		None,
		Blink,
		FadeIn,
		FadeOut,
		FadeScaleOut,
		Display
	}
	
	[SerializeField] UILabel label;
	[SerializeField] float blinkOnDelay;
	[SerializeField] float blinkOffDelay;
	[SerializeField] float fadeDelay;
	[SerializeField] float fadeScale;
			
	private State mCurState = State.None;
	private Color mDefaultColor;
	private float mCurTime;
	private Vector3 mDefaultScale;
	
	public Color color {
		get {
			return mDefaultColor;
		}
		set {
			label.color = mDefaultColor = value;
		}
	}
	
	public State state {
		get {
			return mCurState;
		}
		set {
			mCurState = value;
			ResetData();
			
			switch(mCurState) {
			case State.None:
				label.enabled = false;
				break;
			case State.FadeIn:
				label.enabled = true;
				Color c = label.color; c.a = 0.0f;
				label.color = c;
				break;
			case State.Display:
				label.enabled = true;
				ResetData();
				break;
				
			case State.Blink:
			case State.FadeOut:
			case State.FadeScaleOut:
				label.enabled = true;
				break;
			}
		}
	}
	
	public string message {
		get {
			return label.text;
		}
		set {
			label.text = value;
		}
	}
	
	void ResetData() {
		label.color = mDefaultColor;
		label.transform.localScale = mDefaultScale;
		mCurTime = 0.0f;
	}
	
	void Awake() {
		mDefaultColor = label.color;
		mDefaultScale = label.transform.localScale;
		state = State.None;
	}
	
	// Update is called once per frame
	void Update () {
		switch(mCurState) {
		case State.Blink:
			mCurTime += Time.deltaTime;
			float delay = label.enabled ? blinkOnDelay : blinkOffDelay;
			if(mCurTime >= delay) {
				mCurTime = 0.0f;
				label.enabled = !label.enabled;
			}
			break;
		case State.FadeIn:
			mCurTime += Time.deltaTime;
			if(mCurTime >= fadeDelay) {
				state = State.Display;
			}
			else {
				float t = Ease.In(mCurTime, fadeDelay, 0.0f, 1.0f);
				Color c = label.color;
				c.a = t;
				label.color = c;
			}
			break;
		case State.FadeOut:
		case State.FadeScaleOut:
			mCurTime += Time.deltaTime;
			if(mCurTime >= fadeDelay) {
				state = State.None;
			}
			else {
				float t = Ease.Out(mCurTime, fadeDelay, 0.0f, 1.0f);
				
				Color c = label.color;
				c.a = 1.0f-t;
				label.color = c;
				
				if(mCurState == State.FadeScaleOut) {
					Vector3 s = mDefaultScale;
					s.x = s.y = s.x + s.x*fadeScale*t;
					label.transform.localScale = s;
				}
			}
			break;
		}
	}
}
