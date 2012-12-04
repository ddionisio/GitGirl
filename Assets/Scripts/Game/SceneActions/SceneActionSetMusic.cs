using UnityEngine;
using System.Collections;

public class SceneActionSetMusic  : SequencerAction {
	public string music;
	
	public bool immediate=false;
	
	public bool waitForPlay=true;

	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {	
		MusicManager.instance.Play(music, immediate);
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		return !waitForPlay || MusicManager.instance.IsPlaying();
	}
}