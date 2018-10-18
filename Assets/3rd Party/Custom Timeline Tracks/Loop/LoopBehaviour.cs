using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class LoopBehaviour : PlayableBehaviour
{
	public PlayableDirector director { get; set; }

	public WaitTimeline waitTimeline { get; set; }

	public override void OnBehaviourPause (Playable playable, FrameData info)
	{
		if (waitTimeline == null) return;
		if (waitTimeline.trigger) {
			waitTimeline.trigger = false;
			return;
		}
		director.time -= playable.GetDuration();
	}
}
