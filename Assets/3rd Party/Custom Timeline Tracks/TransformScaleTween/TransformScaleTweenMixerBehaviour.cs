using System;
using UnityEngine;
using UnityEngine.Playables;

public class TransformScaleTweenMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if(trackBinding == null)
            return;

        Vector3 defaultPosition = trackBinding.position;
        Quaternion defaultRotation = trackBinding.rotation;
        Vector3 defaultScale = trackBinding.localScale;

        int inputCount = playable.GetInputCount ();

        float positionTotalWeight = 0f;
        float rotationTotalWeight = 0f;
        float scaleTotalWeight = 0f;

        Vector3 blendedPosition = Vector3.zero;
        Quaternion blendedRotation = new Quaternion(0f, 0f, 0f, 0f);
        Vector3 blendedScale = Vector3.one;

        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<TransformScaleTweenBehaviour> playableInput = (ScriptPlayable<TransformScaleTweenBehaviour>)playable.GetInput (i);
            TransformScaleTweenBehaviour input = playableInput.GetBehaviour ();

            if(input.endLocation == null)
                continue;

            float inputWeight = playable.GetInputWeight(i);

            if (!m_FirstFrameHappened && !input.startLocation)
            {
                input.startingPosition = defaultPosition;
                input.startingRotation = defaultRotation;
                input.startingScale = defaultScale;
            }

            float normalisedTime = (float)(playableInput.GetTime() / playableInput.GetDuration ());
            float tweenProgress = input.EvaluateCurrentCurve(normalisedTime);

            if (input.tweenPosition)
            {
                positionTotalWeight += inputWeight;

                blendedPosition += Vector3.Lerp(input.startingPosition, input.endLocation.position, tweenProgress) * inputWeight;
            }

            if (input.tweenRotation)
            {
                rotationTotalWeight += inputWeight;

                Quaternion desiredRotation = Quaternion.Lerp(input.startingRotation, input.endLocation.rotation, tweenProgress);
                desiredRotation = NormalizeQuaternion(desiredRotation);

                if (Quaternion.Dot (blendedRotation, desiredRotation) < 0f)
                {
                    desiredRotation = ScaleQuaternion (desiredRotation, -1f);
                }

                desiredRotation = ScaleQuaternion(desiredRotation, inputWeight);

                blendedRotation = AddQuaternions (blendedRotation, desiredRotation);
            }
            
            if (input.tweenScale)
            {
                scaleTotalWeight += inputWeight;

                blendedScale += Vector3.Lerp(input.startingScale, input.endLocation.localScale, tweenProgress) * inputWeight;
            }
        }

        blendedPosition += defaultPosition * (1f - positionTotalWeight);
        Quaternion weightedDefaultRotation = ScaleQuaternion (defaultRotation, 1f - rotationTotalWeight);
        blendedRotation = AddQuaternions (blendedRotation, weightedDefaultRotation);
        blendedScale += defaultScale * (1f - scaleTotalWeight);

        trackBinding.position = blendedPosition;
        trackBinding.rotation = blendedRotation;
        trackBinding.localScale = blendedScale - Vector3.one;
        
        m_FirstFrameHappened = true;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_FirstFrameHappened = false;
    }

    static Quaternion AddQuaternions (Quaternion first, Quaternion second)
    {
        first.w += second.w;
        first.x += second.x;
        first.y += second.y;
        first.z += second.z;
        return first;
    }

    static Quaternion ScaleQuaternion (Quaternion rotation, float multiplier)
    {
        rotation.w *= multiplier;
        rotation.x *= multiplier;
        rotation.y *= multiplier;
        rotation.z *= multiplier;
        return rotation;
    }

    static float QuaternionMagnitude (Quaternion rotation)
    {
        return Mathf.Sqrt ((Quaternion.Dot (rotation, rotation)));
    }

    static Quaternion NormalizeQuaternion (Quaternion rotation)
    {
        float magnitude = QuaternionMagnitude (rotation);

        if (magnitude > 0f)
            return ScaleQuaternion (rotation, 1f / magnitude);

        Debug.LogWarning ("Cannot normalize a quaternion with zero magnitude.");
        return Quaternion.identity;
    }
}