using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AttackHinterscript : MonoBehaviour
{
    public static Object AttackHinterPerfab = null;

    public float hintTime = 1.2f;
    public float spaceTime = 0.15f;
    public float liveTime = 1.0f;

    AudioSource audioSource;

    private float spawnTime;
    private float activeTime;
    private float endTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GetComponent<MeshRenderer>().enabled = true;
        spawnTime = Time.time;
        activeTime = spawnTime + hintTime + spaceTime;
        endTime = activeTime + liveTime;
        audioSource = GetComponent<AudioSource>();
        if (hintTime <= audioSource.clip.length)
            audioSource.time = audioSource.clip.length - hintTime;
        else
            audioSource.pitch = audioSource.clip.length / hintTime;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= endTime)
            Object.Destroy(gameObject);
    }

    public static GameObject NewPlayer(
    AudioClip audioClip,
     float hintTime = 1.2f,
     float spaceTime = 0.15f,
     float liveTime = 1.0f)
    {
        if (AttackHinterPerfab == null)
            AttackHinterPerfab = Resources.Load<GameObject>("AttackHinter");
        GameObject self = (GameObject)Instantiate(AttackHinterPerfab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
        AudioSource audioSource = self.GetComponent<AudioSource>();
        audioSource.generator = audioClip;
        AttackHinterscript attackHinterscript = self.GetComponent<AttackHinterscript>();
        attackHinterscript.hintTime = hintTime;
        attackHinterscript.spaceTime = spaceTime;
        attackHinterscript.liveTime = liveTime;
        return self;
    }
}
