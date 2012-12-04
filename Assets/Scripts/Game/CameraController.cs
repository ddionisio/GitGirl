using UnityEngine;
using System.Collections;

//make sure to set the game object tag to MainCamera
public class CameraController : MonoBehaviour {
	public enum Mode {
		Free,
		Attach
	}
	
	public float moveDelay = 0.0f;
	
	private static CameraController mInstance = null;
	
	private Mode mCurMode = Mode.Free;
	
	private Transform mAttach;
	
	private Transform mOrigin;
	
	private float mOriginMinDistance;
	
	private Vector3 prevPos;
	private Quaternion prevRot;
	private float mCurTime;
	
	//prev pos, prev up
	//curTime
	
	//public Anchor.Type anchor = Anchor.Type.BottomLeft;
	
	//private tk2dCamera mTKCamera;
	
	public static CameraController instance {
		get {
			return mInstance;
		}
	}
	
	public Transform origin {
		get {
			return mOrigin;
		}
		set {
			mOrigin = value;
		}
	}
	
	public Transform attach {
		get {
			return mAttach;
		}
		set {
			mAttach = value;
			SetPrev();
		}
	}
	
	public Mode mode {
		get {
			return mCurMode;
		}
		set {
			mCurMode = value;
			
			switch(mCurMode) {
			case Mode.Free:
				break;
			case Mode.Attach:
				break;
			}
			
			//TODO: set interpolation
		}
	}
	
	public bool isMoveFinish {
		get {
			return mCurTime >= moveDelay;
		}
	}
	
	public void CancelMove() {
		mCurTime = moveDelay;
	}
	
	public void Reset() {
		mAttach = null;
		mOrigin = null;
		mCurMode = Mode.Free;
		transform.localPosition = new Vector3(0,0,transform.localPosition.z);
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}
	
	public float originMinDistance {
		get {
			return mOriginMinDistance;
		}
		set {
			mOriginMinDistance = value;
		}
	}
	
	void SceneShutdown() {
		Reset();
	}
	
	void OnDestroy() {
		mInstance = null;
	}
	
	void Awake() {
		mInstance = this;
		//mTKCamera = mCamera.GetComponent<tk2dCamera>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(mCurMode) {
		case Mode.Attach:
			if(mAttach != null) {
				Vector3 attachPos = mAttach.position;
				
				Transform camTrans = transform;
				Vector3 camPos = camTrans.localPosition;
				
				if(mCurTime < moveDelay) {
					float t;
					
					mCurTime += Time.deltaTime;
					if(mCurTime >= moveDelay) {
						t = 1.0f;
					}
					else {
						t = Ease.In(mCurTime, moveDelay, 0.0f, 1.0f);
					}
					
					attachPos.z = camPos.z;
					
					Vector3 newPos = Vector3.Lerp(prevPos, attachPos, t);
					
					//limit the position's distance to origin
					LimitFromOrigin(camPos, ref newPos);
					
					camTrans.localPosition = newPos;
					camTrans.localRotation = Quaternion.Slerp(prevRot, mAttach.rotation, t);
				}
				else if(attachPos.x != camPos.x || attachPos.y != camPos.y) {
					camTrans.localRotation = mAttach.rotation;
					
					Vector3 newPos = new Vector3(attachPos.x, attachPos.y, camPos.z);//new Vector3(attachPos.x + tX, attachPos.y + tY, camPos.z);
					
					//limit the position's distance to origin
					LimitFromOrigin(camPos, ref newPos);
					
					camTrans.localPosition = newPos;
				}
			}
			break;
				
		case Mode.Free:
			if(mOrigin != null) {
				Transform camTrans = transform;
				Vector3 camPos = camTrans.localPosition;
				LimitFromOrigin(camPos, ref camPos);
				camTrans.localPosition = camPos;
			}
			break;
		}
	}
	
	void LimitFromOrigin(Vector3 camPos, ref Vector3 newPos) {
		//limit the position's distance to origin
		if(mOrigin != null) {
			Vector3 origPos = mOrigin.position;
			Vector3 dirToCam = newPos - origPos;
			float dirToCamDist = dirToCam.magnitude;
			if(dirToCamDist < mOriginMinDistance) {
				dirToCam /= dirToCamDist;
				newPos = origPos + dirToCam*mOriginMinDistance;
				newPos.z = camPos.z;
			}
		}
	}
	
	void SetPrev() {
		prevPos = transform.localPosition;
		prevRot = transform.localRotation;
		mCurTime = 0;
	}
}
