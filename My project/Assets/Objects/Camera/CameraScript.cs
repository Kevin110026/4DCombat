using System.Collections;
// using System.Numerics;
using UnityEngine;

// [RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField]
    float _fieldOfView = 0;
    // [SerializeField]
    public float fieldOfView
    {
        get
        {
            return _fieldOfView;
        }
        set
        {
            if (cam != null)
                cam.fieldOfView += value - _fieldOfView;
            _fieldOfView = value;
        }
    }
    Camera cam;
    void Awake()
    {
        cam = transform.GetChild(0).GetComponent<Camera>();
        cam.fieldOfView = fieldOfView;
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
        Vector3 rot = cam.transform.localRotation.eulerAngles;
        rot.z += rotDelta;
        cam.transform.localRotation = Quaternion.Euler(rot);

        yield return new WaitForSeconds(stallTime);
        float startTime = Time.time;
        float d = 0;
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            float k = (2 - t) * t;

            rot = cam.transform.localRotation.eulerAngles;
            rot.z -= rotDelta * k - d;
            cam.transform.localRotation = Quaternion.Euler(rot);

            d = rotDelta * k;
            yield return null;
        }
        rot = cam.transform.localRotation.eulerAngles;
        rot.z -= rotDelta - d;
        cam.transform.localRotation = Quaternion.Euler(rot);
    }
    public void ScreenShakeRotSHM(float amplitude, float frequency, float gamma, float number)
    {
        StartCoroutine(ScreenShakeRotSHMIEnumerator(amplitude, frequency, gamma, number));
    }

    IEnumerator ScreenShakeRotSHMIEnumerator(float amplitude, float frequency, float gamma, float number)
    {
        // Debug.Log(amplitude + ":" + frequency + ":" + gamma + ":" + number);
        Vector3 rot;
        float d = 0;
        float duration = number / frequency;
        float startTime = Time.time;
        // Debug.Log(duration);
        while (Time.time < startTime + duration)
        {
            float t = Time.time - startTime;
            float x = amplitude * Mathf.Exp(-gamma * t) * Mathf.Cos(Mathf.PI * 2 * t * frequency);
            // Debug.Log(x);

            rot = cam.transform.localRotation.eulerAngles;
            rot.z += x - d;
            // Debug.Log(rot);
            cam.transform.localRotation = Quaternion.Euler(rot);

            d = x;
            yield return null;
        }
        rot = cam.transform.localRotation.eulerAngles;
        rot.z += -d;
        cam.transform.localRotation = Quaternion.Euler(rot);
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

    public WaveAlongObject_Obj WaveAlongObject(GameObject obj, float time = 0.125f, float mag = 1f, float mult = 0.06f)
    {
        WaveAlongObject_Obj ret = gameObject.AddComponent<WaveAlongObject_Obj>();
        ret.Init(obj, this, time, mag, mult);
        return ret;
    }

    public class WaveAlongObject_Obj : MonoBehaviour
    {
        CameraScript cameraScript;
        GameObject targetObj;
        const float t_p = 0.8f;
        float fm = 1;
        float time = 0.125f;
        float mag = 3;
        float mult = 0.1f;
        float fx(float x)
        {
            return fm * x / (1 - x);
        }
        float ft(float t)
        {
            return fx(t / (time / t_p));
        }
        float rfx(float x)
        {
            x /= fm;
            return x / (1 + x);
        }
        float rft(float x)
        {
            return rfx(x) * (time / t_p);
        }
        public void Init(GameObject obj, CameraScript cameraScript, float time, float mag, float mult)
        {
            this.targetObj = obj;
            this.cameraScript = cameraScript;
            this.time = time;
            this.mag = mag;
            this.mult = mult;
        }

        public void setTargetObj(GameObject targetObj)
        {
            this.targetObj = targetObj;
            if (targetObj != null)
                objLocPos = targetObj.transform.position - cameraScript.transform.position;
        }

        // public WaveAlongObject_Obj(GameObject obj, CameraScript cameraScript)
        // {
        //     targetObj = obj;
        //     this.cameraScript = cameraScript;
        // }
        // public WaveAlongObject_Obj(GameObject obj, CameraScript cameraScript, float t, float m) : this(obj, cameraScript)
        // {
        //     this.t = Mathf.Max(t, 0.001f);
        //     this.m = Mathf.Max(m, 0.001f);
        // }

        Quaternion _rotation = Quaternion.identity;
        Quaternion Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                cameraScript.cam.transform.rotation *= value * Quaternion.Inverse(_rotation);
                _rotation = value;
            }
        }
        Vector3 objLocPos;
        Vector3 objLocSpeed;
        bool parryMode = false;
        float parryDuration;
        float parryStartTime;
        void Start()
        {
            fm = 1;
            fm = mag / fx(t_p);
            objLocPos = targetObj.transform.position - cameraScript.transform.position;
        }

        void Update()
        {
            // Debug.Log("Rot: " + Rotation);
            //  pull back first
            float angle = Quaternion.Angle(Quaternion.identity, Rotation);
            if (angle != 0)
            {
                float t_angle = rft(angle);
                t_angle = Mathf.Max(t_angle - Time.deltaTime, 0);
                float target_angle = ft(t_angle);
                Rotation = Quaternion.Slerp(Quaternion.identity, Rotation, target_angle / angle);
                Debug.Log("x: " + t_angle / time);
                Debug.Log("angle: " + angle);
            }

            //  and then wave
            if (targetObj != null || parryMode)
            {
                Vector3 objLocPosNew;
                if (targetObj != null)
                {
                    objLocPosNew = targetObj.transform.position - cameraScript.transform.position;
                    objLocSpeed = (objLocPosNew - objLocPos) / Time.deltaTime;
                }
                else
                {
                    float dt;
                    if (Time.time <= parryStartTime + parryDuration)
                    {
                        dt = Time.deltaTime;
                    }
                    else
                    {
                        dt = Time.deltaTime - (Time.time - (parryStartTime + parryDuration));
                        parryMode = false;
                    }
                    objLocPosNew = objLocPos - 10 * dt * objLocSpeed;
                    // Debug.Log("inv");
                }

                Quaternion deltaRot = Quaternion.LookRotation(objLocPosNew) * Quaternion.Inverse(Quaternion.LookRotation(objLocPos));
                objLocPos = objLocPosNew;
                deltaRot = Quaternion.Slerp(Quaternion.identity, deltaRot, mult);
                Rotation *= deltaRot;
            }



            if (angle == 0 && targetObj == null && !parryMode)
            {
                Destroy(this);
            }
        }

        public void Parried(float duration = 0.5f)
        {
            parryMode = true;
            targetObj = null;
            parryStartTime = Time.time;
            parryDuration = duration;
            fm *= 50;
            mult *= 50;
        }

        void OnDestroy()
        {
            Rotation = Quaternion.identity;
        }
    }
    public class WaveTargetDummy : MonoBehaviour
    {
        static public GameObject StraightLine(Vector3 begin, Vector3 end, float time)
        {
            GameObject ret = new GameObject();
            ret.AddComponent<WaveTargetDummy>().Init(begin, end, time);
            return ret;
        }
        Vector3 begin;
        Vector3 end;
        float time;
        float startTime;
        void Init(Vector3 begin, Vector3 end, float time)
        {
            this.begin = begin;
            this.end = end;
            this.time = time;
        }
        void Start()
        {
            startTime = Time.time;
            transform.position = begin;
        }
        void Update()
        {
            float t = (Time.time - startTime) / time;
            t = Mathf.Min(t, 1);
            transform.position = Vector3.Lerp(begin, end, t);
            if (t == 1)
                Destroy(gameObject);
        }
    }
}
