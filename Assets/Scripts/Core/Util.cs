using UnityEngine;
using System.Collections;

public class Util {
	public enum Side {
		None,
		Left,
		Right
	}
	
	/// <summary>
	/// Checks which side up1 is in relation to up2
	/// </summary>
	public static Side CheckSide(Vector2 up1, Vector2 up2) {
		float s = Vector2DCross(up1, up2);
		return s == 0 ? Side.None : s < 0 ? Side.Right : Side.Left;
	}
	
	public static Vector2 Vector2DRot(Vector2 v, float r) {
		float c = Mathf.Cos(r);
		float s = Mathf.Sin(r);
		
		return new Vector2(v.x*c+v.y*s, -v.x*s+v.y*c);
	}
	
	public static float Vector2DCross(Vector2 v1, Vector2 v2) {
		return (v1.x*v2.y) - (v1.y*v2.x);
	}
	
	public static Vector2 MouseToScreen() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
