using UnityEngine;
using System.Collections;

public struct PolarCoord {
	public const float PI_2 = Mathf.PI*2.0f;
	
	public float r;
	
	/// <summary> The angle in radians [0, 2PI). </summary>
	public float theta;
	
	public static PolarCoord zero {
		get {
			return new PolarCoord(0, 0);
		}
	}
	
	public static PolarCoord From(float x, float y) {
		float r, theta;
		
		r = Mathf.Sqrt(x*x + y*y);
		
		if(x == 0 && y == 0) {
			theta = 0;
		}
		else {
			theta = Mathf.Atan2(y, x);
			
			if(theta < 0) {
				theta += PI_2;
			}
		}
						
		return new PolarCoord(r, theta);
	}
	
	public static PolarCoord FromVector(Vector2 v) {
		return From(v.x, v.y);
	}
	
	public static PolarCoord FromVector(Vector3 v) {
		return From(v.x, v.y);
	}
	
	public PolarCoord(float dist, float angle) {
		r = dist;
		theta = angle;
		
		if(theta < 0) {
			theta += PI_2;
		}
	}
	
	public Vector2 ToVector2() {
		return new Vector2(r*Mathf.Cos(theta), r*Mathf.Sin(theta));
	}
	
	public Vector3 ToVector3() {
		return new Vector3(r*Mathf.Cos(theta), r*Mathf.Sin(theta), 0.0f);
	}
	
	public static PolarCoord operator +(PolarCoord p1, PolarCoord p2) {
		float nTheta = p1.theta+p2.theta;
		if(nTheta < 0) {
			nTheta = PI_2 - (nTheta%PI_2);
		}
		else if(nTheta > PI_2) {
			nTheta %= PI_2;
		}
		
		return new PolarCoord(p1.r+p2.r, nTheta);
	}
	
	public static PolarCoord operator -(PolarCoord p1, PolarCoord p2) {
		float nTheta = p1.theta-p2.theta;
		if(nTheta < 0) {
			nTheta = PI_2 - (nTheta%PI_2);
		}
		else if(nTheta > PI_2) {
			nTheta %= PI_2;
		}
		
		return new PolarCoord(p1.r-p2.r, nTheta);
	}
}
