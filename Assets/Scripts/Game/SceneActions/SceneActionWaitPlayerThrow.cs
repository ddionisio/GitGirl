using UnityEngine;
using System.Collections;

//wait for player to throw something
public class SceneActionWaitPlayerThrow : SequencerAction {
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		state.counter = 0;
		
		Player p = ((SceneLevel)behaviour).player;
		p.throwCallback = delegate(Player player) {
			state.counter = 1;
		};
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		return state.counter == 1;
	}
	
	public override void Finish (MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Player p = ((SceneLevel)behaviour).player;
		p.throwCallback = null;
	}
}