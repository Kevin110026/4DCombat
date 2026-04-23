using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerParryBoxscript : MonoBehaviour
{
    private static AudioSource audioSource;
    private float parryStart;
    private float parryCDend;
    public float parryCD = 0.3f;
    public GameObject parryLightObject;
    private Light parryLight;
    public AudioClip[] parrySounds;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        parryLight = parryLightObject.GetComponent<Light>(); ;
    }
    void Start()
    {
        parryStart = Time.time - 100.0f;
        parryCDend = Time.time - 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParry();
        parryLight.intensity = (float)Math.Clamp(parryLight.intensity * Math.Pow(0.01, Time.deltaTime / 0.1f), 0, float.PositiveInfinity);
    }

    void UpdateParry()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Time.time >= parryCDend)
            {
                parryStart = Time.time;
                parryCDend = parryStart + parryCD;
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Attackscript>(out Attackscript attack))
        {
            VerifyParryForAttack(attack);
        }
    }

    public bool VerifyParryForAttack(Attackscript attack)
    {
        if (Time.time <= parryStart + 0.3f && attack.canBeParried)
        {
            OnParry(attack);
            attack.OnParried(transform.parent.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnParry(Attackscript attack)
    {
        audioSource.pitch = attack.parriedPitch;
        if (attack.parriedSound != null)
            audioSource.PlayOneShot(attack.parriedSound);
        else
            audioSource.PlayOneShot(audioSource.clip);
        parryCDend = Time.time;
        parryLight.intensity = 100f;
    }
}
