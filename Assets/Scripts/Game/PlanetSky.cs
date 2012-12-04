using UnityEngine;
using System.Collections;

//put in planet object
public class PlanetSky : MonoBehaviour {
	public Color spaceColor;
	public Color skyColor;
	
	public float skyDistance; //from planet, once distance to cam > skyDistance, start interpolation
	public float spaceDistance; //interpolation of distance between sky and space, cam distance > space is completely space
	
	private enum State {
		InBetween,
		InSpace,
		InSky
	}
	
	private State mCurState = State.InBetween;
	
	private float mSkyDistanceSq;
	private float mSpaceDistanceSq;
	private float mDeltaDistanceSq;
	
	private Vector3 mPos;
	private Transform mCamTrans;
	
	void Awake() {
		mSkyDistanceSq = skyDistance*skyDistance;
		mSpaceDistanceSq = spaceDistance*spaceDistance;
		mDeltaDistanceSq = mSpaceDistanceSq-mSkyDistanceSq;
		mCamTrans = Camera.main.transform;
	}
	
	// Use this for initialization
	void Start () {
		mPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float distSq = (mCamTrans.position - mPos).sqrMagnitude;
		if(distSq >= mSpaceDistanceSq) {
			if(mCurState != State.InSpace) {
				mCurState = State.InSpace;
				Camera.main.backgroundColor = spaceColor;
			}
		}
		else if(distSq <= mSkyDistanceSq) {
			if(mCurState != State.InSky) {
				mCurState = State.InSky;
				Camera.main.backgroundColor = skyColor;
			}
		}
		else {
			mCurState = State.InBetween;
			Camera.main.backgroundColor = Color.Lerp(skyColor, spaceColor, (distSq-mSkyDistanceSq)/mDeltaDistanceSq);
		}
	}
}
