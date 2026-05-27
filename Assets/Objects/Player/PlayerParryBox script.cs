using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PlayerParryBoxscript : MonoBehaviour
{
    private static AudioSource audioSource;
    private float parryStart;
    private float parryCDend;
    public float parryCD = 0.15f;
    public float parryWindow = 0.3f;
    public GameObject parryLightObject;
    private Light parryLight;
    public AudioClip[] parrySounds;
    private LinkedList<float> parryBuffer;
    private float blockBufferTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        parryLight = parryLightObject.GetComponent<Light>();
        parryBuffer = new LinkedList<float>();
        blockBufferTime = Time.time - 1000f;
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
        while (parryBuffer.Count > 0 && Time.time > parryBuffer.First.Value + parryWindow)
            parryBuffer.RemoveFirst();
        TryToParryFromBuffer();
    }

    bool TryToParryFromBuffer()
    {
        if (parryBuffer.Count > 0 && TryToParry(parryBuffer.First.Value))
        {
            parryBuffer.RemoveFirst();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool TryToParry(float parryTime)
    {
        if (parryTime >= parryCDend)
        {
            parryStart = parryTime;
            parryCDend = parryStart + parryWindow + parryCD;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddParryToBuffer()
    {
        if (Time.time <= blockBufferTime)
            return;
        parryBuffer.AddLast(Time.time);
        TryToParryFromBuffer();
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
        if (Time.time <= parryStart + parryWindow && attack.canBeParried)
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
        parryCDend = Time.time - 100 * parryWindow;
        parryStart = Time.time - 100 * parryWindow;
        parryLight.intensity = 100f;
        TryToParryFromBuffer();
    }
    public void ResetBufferAndCd()
    {
        parryBuffer.Clear();
        parryCDend = Time.time - 100 * parryWindow;
        parryStart = Time.time - 100 * parryWindow;
        blockBufferTime = Time.time + 0.1f;
    }

    public bool IsParrying()
    {
        return Time.time <= parryStart + parryWindow;
    }
}
