using UnityEngine;
using System.Collections;

public class SceneActionActivateEntityType : SequencerAction {
	public string type;
	public bool activate = true;
	public bool fromScene = false; //use the entire scene for search, otherwise, just in entities
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(type.Length > 0) {
			System.Type theType = System.Type.GetType(type);
			
			Component[] comps;
			if(fromScene) {
				comps = ((SceneController)behaviour).GetComponentsInChildren(theType);
			}
			else {
				comps = EntityManager.instance.GetComponentsInChildren(theType);
			}
			
			foreach(Component c in comps) {
				c.SendMessage("OnSceneActivate", activate, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}