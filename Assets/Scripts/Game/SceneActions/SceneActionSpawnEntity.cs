using UnityEngine;
using System.Collections;

public class SceneActionSpawnEntity : SequencerAction {
	public string type;
	public string name;
	public string waypoint;
	public bool useFX=false;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		EntityManager.instance.Spawn(type, name, null, waypoint, useFX);
	}
}
