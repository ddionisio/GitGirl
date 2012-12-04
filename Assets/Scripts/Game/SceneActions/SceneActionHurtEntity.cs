using UnityEngine;
using System.Collections;

public class SceneActionHurtEntity : SequencerAction {
	public string entityPath = "";
	public int amount = 0; //-1 to set hp to 1
	public Entity.Action action = Entity.Action.hurt;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(entityPath.Length > 0) {
			GameObject go = ((SceneController)behaviour).SearchObject(entityPath);
			if(go != null) {
				Entity e = go.GetComponent<Entity>();
				if(e != null) {
					if(e.stats != null) {
						if(amount == -1) {
							if(e.stats.curHP > 1) {
								e.stats.ApplyDamage(e.stats.curHP-1);
							}
						}
						else {
							e.stats.ApplyDamage(amount);
						}
					}
					
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
