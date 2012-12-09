using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGrabberBase : MonoBehaviour {
	public enum State {
		None,
		Grabbing,
		Grabbed,
		Retracting,
		Retracted,
		Holding //holding an object
	}
	
	public Transform headAttach; //attachment point
	
	public float radius = 10.0f;
	
	public float grabLenOfs = 0.0f;
	public float grabDelay = 0.15f;
	
	//stuff
	protected float mGrabCurDelay = 0.0f;
	protected Transform mGrabTarget = null;
	
	protected bool mRetractIsAttached;
	protected Vector3 mGrabDest;
	
	private State mCurState = State.None;
	
	public State state {
		get {
			return mCurState;
		}
	}
	
	public Player player {
		get {
			return SceneLevel.instance.player;
		}
	}
	
	public virtual Vector3 up {
		get {
			return transform.up;
		}
	}
	
	//call within function OnGrabDone if you want to keep the target
	public void Retract(bool attachGrabbedTarget) {
		mRetractIsAttached = attachGrabbedTarget;
		SwitchState(State.Retracting);
	}
	
	
	
	//
	//
	
	/// <summary>
	/// You better know what to do with the grabbed item, parent is set to null.
	/// e.g. you can move it back to a pool of some sort.
	/// </summary>
	/// <returns>
	/// The detached object, parentless. :(
	/// </returns>
	public Transform DetachGrab() {
		SwitchState(State.None);
		
		Transform ret = mGrabTarget;
		if(ret != null) {
			ret.parent = null;
			
			ret.SendMessage("OnGrabDetach", this, SendMessageOptions.DontRequireReceiver);
		}
		
		mGrabTarget = null;
		mRetractIsAttached = false;
						
		return ret;
	}
	
	protected virtual void Start () {
		//Input.mousePosition
		SwitchState(State.None);
	}
	
	protected virtual void SwitchState(State newState) {
		mCurState = newState;
		mGrabCurDelay = 0.0f;
		
		Player thePlayer = player;
		
		switch(newState) {
		case State.None:
			break;
			
		case State.Grabbing:
			thePlayer.OnGrabStart(this);
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabStart", this, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case State.Grabbed:
			thePlayer.OnGrabDone(this);
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabDone", this, SendMessageOptions.DontRequireReceiver);
			}
			
			//wait for action on grabbed target
			break;
			
		case State.Retracting:
			if(mGrabTarget != null) {
				mGrabDest = mGrabTarget.position;
				
				//move grabbed object to attach point
				if(mRetractIsAttached) {
					mGrabTarget.parent = headAttach;
					mGrabTarget.localPosition = new Vector3(0, 0, mGrabDest.z);
					mGrabTarget.localScale = Vector3.one;
					mGrabTarget.localRotation = Quaternion.identity;
				}
			}
			else {
				mGrabDest = transform.position;
			}
			
			thePlayer.OnGrabRetractStart(this);
			
			if(mGrabTarget != null) {
				mGrabTarget.SendMessage("OnGrabRetractStart", this, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case State.Retracted:
			thePlayer.OnGrabRetractEnd(this);
			
			if(mRetractIsAttached && mGrabTarget != null) { //don't care if grabbed is left alone
				mGrabTarget.SendMessage("OnGrabRetractEnd", this, SendMessageOptions.DontRequireReceiver);
			}
			else {
				mGrabTarget = null;
			}
			
			SwitchState(State.None);
			break;
		}
	}
}
