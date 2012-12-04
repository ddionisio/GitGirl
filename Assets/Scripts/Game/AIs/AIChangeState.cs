using UnityEngine;
using System.Collections;

public class AIChangeState : SequencerAction {
	public string toState;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		if(!string.IsNullOrEmpty(toState)) {
			((Entity)behaviour).AISetState(toState);
		}
	}
}
