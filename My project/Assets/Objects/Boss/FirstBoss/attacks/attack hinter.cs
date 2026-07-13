using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class attackplayer : MonoBehaviour
{
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
        GetComponent<MeshRenderer>().enabled = false;
        // GetComponent<MeshRenderer>().enabled = true;
        spawnTime = Time.time;
        activeTime = spawnTime + hintTime + spaceTime;
        endTime = activeTime + liveTime;
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = audioSource.clip.length / hintTime;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= activeTime)
            GetComponent<MeshRenderer>().enabled = true;
        if(Time.time >= endTime)
            Object.Destroy(gameObject);
    }
}
