using UnityEngine;

public class AudioPlayer: MonoBehaviour
{
    public static GameObject PlayAudio(AudioClip clip, float pitch, float offset)
    {
        GameObject obj = new GameObject();
        AudioSource aus = obj.AddComponent<AudioSource>();
        obj.AddComponent<AudioPlayer>();
        aus.clip = clip;
        aus.pitch = pitch;
        aus.time = offset;
        aus.Play();
        return obj;
    }

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (audioSource.time >= audioSource.clip.length)
            Destroy(gameObject);
    }
}
