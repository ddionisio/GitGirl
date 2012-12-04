using UnityEngine;
using System.Collections;

public class SceneActionWaitEntityAction : SequencerAction {
	public string entityPath = "";
	public Entity.Action action;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(entityPath.Length > 0) {
			GameObject go = ((SceneController)behaviour).SearchObject(entityPath);
			if(go != null) {
				Entity e = go.GetComponent<Entity>();
				if(e != null) {
					((SceneController.StateData)state).entity = e;
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
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		SceneController.StateData dat = (SceneController.StateData)state;
		
		bool done = true;
		
		if(dat.entity != null) {
			done = dat.entity.action == action;
		}
		
		return done;
	}
	
	//do clean ups here, don't set any states dependent from outside
	public override void Finish(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		((SceneController.StateData)state).entity = null;
	}
}