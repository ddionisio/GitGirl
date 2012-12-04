using UnityEngine;
using System.Collections;

public class SceneActionWaitEntityCounter : SequencerAction {	
	public string entityPath = "";
	public int counter = 1;
			
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(entityPath.Length > 0) {
			GameObject go = ((SceneController)behaviour).SearchObject(entityPath);
			if(go != null) {
				Entity e = go.GetComponent<Entity>();
				if(e != null) {
					state.counter = 0;
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
	
	public override bool Update (MonoBehaviour behaviour, Sequencer.StateInstance state) {
		bool done = true;
		
		Entity e = ((SceneController.StateData)state).entity;
		if(e != null) {
			e.SendMessage("OnSceneCounterCheck", state, SendMessageOptions.RequireReceiver);
			done = state.counter == counter;
		}
		
		return done;
	}
	
	public override void Finish (MonoBehaviour behaviour, Sequencer.StateInstance state) {
		((SceneController.StateData)state).entity = null;
	}
}
