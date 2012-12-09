using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	//only use these after awake
	public static int layerIgnoreRaycast;
	public static int layerPlayer;
	public static int layerEnemy;
	public static int layerEnemyComplex;
	public static int layerProjectile;
	public static int layerItem;
	public static int layerPlayerProjectile;
	public static int layerEnemyNoPlayerProjectile;
	public static int layerPlanet;
	public static int layerAutoGrab;
	
	public static int layerMaskPlayer;
	public static int layerMaskEnemy;
	public static int layerMaskEnemyComplex;
	public static int layerMaskProjectile;
	public static int layerMaskItem;
	public static int layerMaskPlayerProjectile;
	public static int layerMaskEnemyNoPlayerProjectile;
	public static int layerMaskPlanet;
	public static int layerMaskAutoGrab;
	
	public TextAsset stringAsset;
	
	[System.NonSerialized] public UserSettings userSettings;
	[System.NonSerialized] public UserData userData;
	[System.NonSerialized] public SceneManager sceneManager;
	[System.NonSerialized] public ReticleManager reticleManager;
	
	private static Main mInstance = null;
	
	private Dictionary<string, object> mStringTable = null;
	
	public static Main instance {
		get {
			return mInstance;
		}
	}
	
	public Dictionary<string, object> strings {
		get {
			if(mStringTable == null) {
				InitStrings();
			}
			
			return mStringTable;
		}
	}
	
	public SceneController sceneController {
		get {
			return sceneManager.sceneController;
		}
	}
	
	void OnApplicationQuit() {
		mInstance = null;
	}
	
	void OnEnable() {
	}
			
	void Awake() {
		mInstance = this;
		
		layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
		layerPlayer = LayerMask.NameToLayer("Player");
		layerEnemy = LayerMask.NameToLayer("Enemy");
		layerEnemyComplex = LayerMask.NameToLayer("EnemyComplex");
		layerProjectile = LayerMask.NameToLayer("Projectile");
		layerItem = LayerMask.NameToLayer("Item");
		layerPlayerProjectile = LayerMask.NameToLayer("PlayerProjectile");
		layerEnemyNoPlayerProjectile = LayerMask.NameToLayer("EnemyNoPlayerProjectile");
		layerPlanet = LayerMask.NameToLayer("Planet");
		layerAutoGrab = LayerMask.NameToLayer("AutoGrab");
		
		layerMaskPlayer = 1<<layerPlayer;
		layerMaskEnemy = 1<<layerEnemy;
		layerMaskEnemyComplex = 1<<layerEnemyComplex;
		layerMaskProjectile = 1<<layerProjectile;
		layerMaskItem = 1<<layerItem;
		layerMaskPlayerProjectile = 1<<layerPlayerProjectile;
		layerMaskEnemyNoPlayerProjectile = 1<<layerEnemyNoPlayerProjectile;
		layerMaskPlanet = 1<<layerPlanet;
		layerMaskAutoGrab = 1<<layerAutoGrab;
		
		DontDestroyOnLoad(gameObject);
		
		userData = GetComponentInChildren<UserData>();
		userSettings = GetComponentInChildren<UserSettings>();
		
		sceneManager = GetComponentInChildren<SceneManager>();
		reticleManager = GetComponentInChildren<ReticleManager>();
		//uiManager = GetComponentInChildren<UIManager>();
		
		InitStrings();
	}
	
	void Start() {
		//TODO: maybe do other things before starting the game
		//go to start if we are in main scene
		SceneManager.Scene mainScene = SceneManager.Scene.main;
		if(Application.loadedLevelName == mainScene.ToString()) {
			sceneManager.LoadScene(SceneManager.Scene.start);
		}
		else {
			sceneManager.InitScene();
		}
	}
	
	void InitStrings() {
		if(mStringTable == null) {
			if(stringAsset != null) {
				mStringTable = fastJSON.JSON.Instance.Parse(stringAsset.text) as Dictionary<string, object>;
			}
		}
	}
}
