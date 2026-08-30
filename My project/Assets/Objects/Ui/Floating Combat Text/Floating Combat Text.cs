using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    static Object prefab;
    public float liveTime = 1f;
    public float fadeTime = 0.2f;
    public float upDistance = 1f;

    float _upDis = 0;

    float spawnTime;
    void Start()
    {
        spawnTime = Time.time;
        fadeTime = Mathf.Min(fadeTime, liveTime);
    }
    void Update()
    {
        if (Time.time - spawnTime > liveTime)
            Destroy(gameObject);
        textMeshPro.alpha = Mathf.Min(Mathf.InverseLerp(liveTime, fadeTime, Time.time - spawnTime), 1);
        Vector3 position = transform.position;
        float k = (Time.time - spawnTime) / liveTime;
        float d = (2 - k) * k * upDistance;
        position.y += d - _upDis;
        _upDis = d;
        transform.position = position;
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.allCameras[0].transform.position);
    }

    static public GameObject NewFCT(string text)
    {
        if (prefab == null)
            prefab = Resources.Load<GameObject>("Floating Combat Text");
        GameObject fct = (GameObject)Instantiate(prefab);
        FloatingCombatText floatingCombatText = fct.GetComponent<FloatingCombatText>();
        floatingCombatText.textMeshPro.text = text;
        return fct;
    }
}
