using UnityEngine;

[CreateAssetMenu(fileName = "StraightLineHintSoundPath", menuName = "Scriptable Objects/HintSoundPath/StraightLineHintSoundPath")]
class StraightLineHintSoundPath : HintSoundPath
{
    public Vector3 start;
    public Vector3 end;

    public override Vector3 Pos(float t)
    {
        return Vector3.Lerp(start, end, t);
    }
}