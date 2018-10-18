using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformScaleTweenClip : PlayableAsset, ITimelineClipAsset
{
    public TransformScaleTweenBehaviour template = new TransformScaleTweenBehaviour ();
    public ExposedReference<Transform> startLocation;
    public ExposedReference<Transform> endLocation;
    
    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformScaleTweenBehaviour>.Create (graph, template);
        TransformScaleTweenBehaviour clone = playable.GetBehaviour ();
        clone.startLocation = startLocation.Resolve (graph.GetResolver ());
        clone.endLocation = endLocation.Resolve (graph.GetResolver ());
        return playable;
    }
}