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

    public void ScreenShakeFov(float fovDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
    {
        StartCoroutine(ScreenShakeFovIEnumerator(fovDelta, stallTime, fadeTime));
    }

    IEnumerator ScreenShakeFovIEnumerator(float fovDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
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
    public void ScreenShakeFovSHM(float amplitude, float frequency, float gamma, float number)
    {
        StartCoroutine(ScreenShakeFovSHMIEnumerator(amplitude, frequency, gamma, number));
    }

    IEnumerator ScreenShakeFovSHMIEnumerator(float amplitude, float frequency, float gamma, float number)
    {
        float d = 0;
        float duration = number / frequency;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = Time.time - startTime;
            float x = amplitude * Mathf.Exp(-gamma * t) * Mathf.Cos(Mathf.PI * 2 * t * frequency);
            cam.fieldOfView += x - d;
            d = x;
            yield return null;
        }
        cam.fieldOfView += -d;
    }
    public void ScreenShakeRot(float rotDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
    {
        StartCoroutine(ScreenShakeRotIEnumerator(rotDelta, stallTime, fadeTime));
    }

    IEnumerator ScreenShakeRotIEnumerator(float rotDelta = -1.5f, float stallTime = 0.05f, float fadeTime = 0.05f)
    {
        Vector3 rot = cam.transform.rotation.eulerAngles;
        rot.z += rotDelta;
        cam.transform.rotation = Quaternion.Euler(rot);

        yield return new WaitForSeconds(stallTime);
        float startTime = Time.time;
        float d = 0;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            float k = (2 - t) * t;

            rot = cam.transform.rotation.eulerAngles;
            rot.z -= rotDelta * k - d;
            cam.transform.rotation = Quaternion.Euler(rot);

            d = rotDelta * k;
            yield return null;
        }
        rot = cam.transform.rotation.eulerAngles;
        rot.z -= rotDelta - d;
        cam.transform.rotation = Quaternion.Euler(rot);
    }
    public void ScreenShakeRotSHM(float amplitude, float frequency, float gamma, float number)
    {
        StartCoroutine(ScreenShakeRotSHMIEnumerator(amplitude, frequency, gamma, number));
    }

    IEnumerator ScreenShakeRotSHMIEnumerator(float amplitude, float frequency, float gamma, float number)
    {
        Vector3 rot;
        float d = 0;
        float duration = number / frequency;
        float startTime = Time.time;
        Debug.Log(duration);
        while (Time.time < startTime + duration)
        {
            float t = Time.time - startTime;
            float x = amplitude * Mathf.Exp(-gamma * t) * Mathf.Cos(Mathf.PI * 2 * t * frequency);

            rot = cam.transform.rotation.eulerAngles;
            rot.z += x - d;
            cam.transform.rotation = Quaternion.Euler(rot);

            d = x;
            yield return null;
        }
        rot = cam.transform.rotation.eulerAngles;
        rot.z += -d;
        cam.transform.rotation = Quaternion.Euler(rot);
    }

    public void FovChangeLinear(float fovDelta, float duration)
    {
        StartCoroutine(FovChangeLinearIEnumerator(fovDelta, duration));
    }

    IEnumerator FovChangeLinearIEnumerator(float fovDelta, float duration)
    {
        float k = fovDelta;
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            float t = Mathf.Sign(fovDelta) * Mathf.Min(Mathf.Abs(k), Time.deltaTime * Mathf.Abs(fovDelta) / duration);
            cam.fieldOfView += t;
            k -= t;
            yield return null;
        }
        cam.fieldOfView += k;
    }

    public void FovChangeQuadratic(float fovDelta, float duration)
    {
        StartCoroutine(FovChangeQuadraticIEnumerator(fovDelta, duration));
    }

    IEnumerator FovChangeQuadraticIEnumerator(float fovDelta, float duration)
    {
        float startTime = Time.time;
        float d = 0;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            float k = (2 - t) * t;
            cam.fieldOfView += fovDelta * k - d;
            d = fovDelta * k;
            yield return null;
        }
        cam.fieldOfView += fovDelta - d;
    }
}
