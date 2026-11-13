using System.Collections;
using UnityEngine;

public class HinterLocateTool
{
    private class HinterLocateTool_DummyRunner : MonoBehaviour { }

    public static GameObject AFaceB(GameObject A, GameObject B, float liveTime = 1f, bool autoDestroy = true)
    {
        GameObject obj = new GameObject("HinterLocateTool AFaceB");
        obj.AddComponent<HinterLocateTool_DummyRunner>().StartCoroutine(AFaceBIEnumerator(obj, A, B, liveTime, autoDestroy));
        return obj;
    }
    static IEnumerator AFaceBIEnumerator(GameObject dummy, GameObject A, GameObject B, float liveTime = 1f, bool autoDestroy = true)
    {
        float startTime = Time.time;
        while (true)
        {
            if (dummy == null || (autoDestroy && Time.time >= startTime + liveTime && dummy.transform.childCount == 0))
                break;
            dummy.transform.position = A.transform.position;
            dummy.transform.rotation = Quaternion.LookRotation(B.transform.position - dummy.transform.position);
            yield return true;
        }
        Object.Destroy(dummy);
    }
}
