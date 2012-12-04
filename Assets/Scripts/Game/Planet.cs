using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
	public float cameraDistanceLimit;
	
	[System.NonSerialized]
	public int enemyCount = 0;
	
	public PlanetBody body {
		get {
			return mPlanetBody;
		}
	}
	
	private PlanetBody mPlanetBody;
	
	void Awake() {
		mPlanetBody = GetComponent<PlanetBody>();
		
		enemyCount = 0;
	}
	
	// Use this for initialization
	void Start () {
		Main.instance.cameraController.originMinDistance = cameraDistanceLimit;
		Main.instance.cameraController.origin = transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
