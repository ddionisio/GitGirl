using UnityEngine;
using System.Collections;

//I'm not new wave.
public class SceneActionNewWave : SequencerAction {
	public bool isBossAnnounce=false;
	
	private enum State {
		AnnounceWave,
		AnnounceGo,
		AnnounceWaitEnd
	}
	
	private const string bossString = "BOSS INCOMING!";
	private const string goString = "LET'S GO!";
	private const float waveDelay = 2;
	private const float goDelay = 0.5f;

	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {	
		SceneLevel.instance.IncWave();
		
		HUDAnnounce announce = UIManager.instance.hud.announce;
		
		announce.state = HUDAnnounce.State.Blink;
		announce.message = isBossAnnounce ? bossString : SceneLevel.instance.GetWaveString();
		announce.color = Color.white;
		
		state.counter = 0;
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		HUDAnnounce announce = UIManager.instance.hud.announce;
		
		bool isDone = false;
		
		switch((State)state.counter) {
		case State.AnnounceWave:
			if(state.IsDelayReached(waveDelay)) {
				if(isBossAnnounce) {
					announce.state = HUDAnnounce.State.None;
					isDone = true;
				}
				else {
					announce.state = HUDAnnounce.State.Display;
					announce.message = goString;
					
					state.startTime = Time.time;
					state.counter++;
				}
			}
			break;
			
		case State.AnnounceGo:
			if(state.IsDelayReached(waveDelay)) {
				announce.state = HUDAnnounce.State.FadeScaleOut;
				
				state.counter++;
			}
			break;
			
		case State.AnnounceWaitEnd:
			isDone = announce.state == HUDAnnounce.State.None;
			break;
		}
		
		return isDone;
	}
}
