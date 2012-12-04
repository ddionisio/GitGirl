using UnityEngine;
using System.Collections;

public class SceneActionEntityAction : SequencerAction {
	public string entityPath = "";
	public Entity.Action action;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(entityPath.Length > 0) {
			GameObject go = ((SceneController)behaviour).SearchObject(entityPath);
			if(go != null) {
				Entity e = go.GetComponent<Entity>();
				if(e != null) {
					e.action = action;
				}
				else {
					Debug.LogWarning("Not an entity: "+entityPath);
				}
			}
			else {
				Debug.LogWarning("Path not found: "+entityPath);
			}
		}
	}
}
