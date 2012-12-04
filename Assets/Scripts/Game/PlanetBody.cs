using UnityEngine;
using System.Collections;

public class PlanetBody : MonoBehaviour {
	public const string planetTag = "Planet";
	
	public float radius = 17.9f;
	public float gravity = -9.8f;
	
	private Vector2 mPos;
	private float mSurfaceLength;
	
	public float surfaceLength {
		get {
			return mSurfaceLength;
		}
	}
			
	public Vector2 ConvertToPlanetPos(Vector2 wPos) {
		Vector2 pos = wPos - mPos;
		PolarCoord polarPos = PolarCoord.FromVector(pos);
		
		return new Vector2((polarPos.theta/PolarCoord.PI_2)*surfaceLength, polarPos.r - radius);
	}
	
	public Vector2 ConvertToWorldPos(Vector2 planetPos) {
		//convert to world space
		PolarCoord polarPos = new PolarCoord(planetPos.y + radius, (planetPos.x/surfaceLength)*PolarCoord.PI_2);
		Vector2 planetBodyPos = transform.position;
		return planetBodyPos + polarPos.ToVector2();
	}
	
	void Awake() {
		tag = planetTag;
		mSurfaceLength = PolarCoord.PI_2*radius;
		mPos = transform.position;
	}
	
	void Update() {
#if UNITY_EDITOR
		mSurfaceLength = PolarCoord.PI_2*radius;
		mPos = transform.position;
#endif
	}
}
