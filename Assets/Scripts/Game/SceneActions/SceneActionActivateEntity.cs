using UnityEngine;
using System.Collections;

public class SceneActionActivateEntity : SequencerAction {
	public string entityPath = "";
	public bool activate = true;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(entityPath.Length > 0) {
			GameObject go = ((SceneController)behaviour).SearchObject(entityPath);
			if(go != null) {
				go.SendMessage("OnSceneActivate", activate, SendMessageOptions.DontRequireReceiver);
			}
			else {
				Debug.LogWarning("Path not found: "+entityPath);
			}
		}
	}
}
