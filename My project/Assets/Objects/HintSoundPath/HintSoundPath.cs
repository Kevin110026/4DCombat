using UnityEngine;

// [CreateAssetMenu(fileName = "HintSoundPath", menuName = "Scriptable Objects/HintSoundPath")]
public class HintSoundPath : ScriptableObject
{
    virtual public Vector3 Pos(float t)
    {
        return Vector3.zero;
    }
}
