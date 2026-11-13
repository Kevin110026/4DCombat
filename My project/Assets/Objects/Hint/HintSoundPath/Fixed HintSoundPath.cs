using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FixedHintSoundPath", menuName = "Scriptable Objects/HintSoundPath/FixedHintSoundPath")]
public class FixedHintSoundPath : HintSoundPath
{
    [SerializeField] Vector3 pos;
    public override Vector3 Pos(float t)
    {
        return pos;
    }
}
