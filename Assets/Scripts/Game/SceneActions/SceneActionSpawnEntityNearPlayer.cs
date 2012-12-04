using UnityEngine;
using System.Collections;

public class SceneActionSpawnEntityNearPlayer : SequencerAction {
	public string type;
	public string name;
	
	//planet position relative to player
	public float x;
	public float y;
	
	public bool useFX=true;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		SceneLevel sl = SceneLevel.instance;
		
		Player player = sl.player;
		
		Transform t = EntityManager.instance.Spawn(type, name, null, null, useFX);
		
		if(t != null) {
			Vector2 playerPos = player.planetAttach.planetPos;
			
			Vector2 pos = sl.planet.body.ConvertToWorldPos(new Vector2(playerPos.x+x, playerPos.y+y));
									
			t.position = new Vector3(pos.x, pos.y, t.position.z);
			t.GetComponentInChildren<PlanetAttachStatic>().RefreshPos();
		}
	}
}