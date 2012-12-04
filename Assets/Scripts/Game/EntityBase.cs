using UnityEngine;
using System.Collections;

public class EntityBase : MonoBehaviour {
	[System.Flags]
	public enum Flag : int {
		None = 0x0,
		Targetted = 0x1,
		Invulnerable = 0x2
	}
	
	protected Reticle.Type mReticle = Reticle.Type.NumType;
	
	private Flag mFlags = Flag.None;
	
	public Reticle.Type reticle {
		get {
			return mReticle;
		}
	}
	
	public void FlagsAdd(Flag flag) {
		mFlags |= flag;
	}
	
	public void FlagsRemove(Flag flag) {
		mFlags ^= flag;
	}
	
	public bool FlagsCheck(Flag flag) {
		return (mFlags & flag) != Flag.None;
	}
	
	//we are being targetted or cleared out of target
	//called by reticle manager when reticle is set to us
	public virtual void OnTargetted(bool yes) {
	}
}
