using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    Camera cam;
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void ScreenShake(float fovDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
    {
        StartCoroutine(ScreenShakeIEnumerator(fovDelta, stallTime, fadeTime));
    }

    IEnumerator ScreenShakeIEnumerator(float fovDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
    {
        cam.fieldOfView += fovDelta;
        yield return new WaitForSeconds(stallTime);
        float startTime = Time.time;
        float d = 0;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            float k = (2 - t) * t;
            cam.fieldOfView -= fovDelta * k - d;
            d = fovDelta * k;
            yield return null;
        }
        cam.fieldOfView -= fovDelta - d;

    }
}
