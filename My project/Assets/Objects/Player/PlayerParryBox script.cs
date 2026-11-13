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
    public float blockEnergyConsumeMultiplier = 2;
    public GameObject parryLightObject;
    private Light parryLight;
    public AudioClip[] parrySounds;
    private LinkedList<float> parryBuffer;
    private float blockBufferTime;

    public PlayerScript playerScript;

    private CharacterScript characterScript;
    private CharacterStateScript characterStateScript;

    public float parryEnergyConsume = 1.5f;

    [HideInInspector] public bool isBlocking = false;

    public AudioClip BlockSoundLight;
    public AudioClip BlockSoundMedium;
    public AudioClip BlockSoundHeavy;
    public AudioClip BlockSoundBreak;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    /*
    want to make it like this: 
    mouse down -> first parry
    mouse hold -> blocking
    mouse up after holding for a while -> second parry

    maybe make the logic in PlayerControlScript
    and here just make Parry and Block
    */

    void Awake()
    {
        // Debug.Log(playerScript);
        // Debug.Log(playerScript.characterScript);
        // Debug.Log(playerScript.characterScript.stateScript);
        audioSource = GetComponent<AudioSource>();
        parryLight = parryLightObject.GetComponent<Light>();
        parryBuffer = new LinkedList<float>();
        blockBufferTime = Time.time - 1000f;
    }

    void Start()
    {
        //  these two run before playerScript is Awaked in Awake, so annoying
        characterScript = playerScript;
        characterStateScript = characterScript.stateScript;

        parryStart = Time.time - 100.0f;
        parryCDend = Time.time - 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParry();
        parryLight.intensity = (float)Math.Clamp(parryLight.intensity * Math.Pow(0.05, Time.deltaTime / 0.1f), 0, float.PositiveInfinity);
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
        if (parryTime >= parryCDend && characterStateScript.BurstEnergy >= parryEnergyConsume)
        {
            parryStart = parryTime;
            parryCDend = parryStart + parryWindow + parryCD;
            characterScript.ConsumeBurstEnergy(parryEnergyConsume);
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
        if (other.TryGetComponent<AttackScript>(out AttackScript attack))
        {
            VerifyParryForAttack(attack);
        }
    }

    public void VerifyParryForAttack(AttackScript attack)
    {
        if (attack.canBeParried)
        {
            if (Time.time <= parryStart + parryWindow)
            {
                OnParry(attack);
            }
            else if (isBlocking)
            {
                BlockAttack(attack);
            }
        }
    }

    void OnParry(AttackScript attack)
    {
        float parryEnergy = parryEnergyConsume * Mathf.Lerp(7f, 2.5f, (Time.time - parryStart) / parryWindow);
        float parriedEnergy = Mathf.Min(attack.energy, parryEnergy);
        attack.energy -= parriedEnergy;
        characterScript.stateScript.BurstEnergy = Mathf.Min(characterScript.stateScript.BurstEnergy + parryEnergyConsume * 0.5f, characterScript.stateScript.BurstEnergyMax);
        // characterScript.ConsumeBurstEnergy(-parryEnergyConsume * 0.5f);
        if (attack.energy == 0)
        {
            attack.OnParried(transform.parent.gameObject);
            parryLight.color = Color.white;
        }
        else
        {
            BlockAttack(attack);
            parryLight.color = Color.orange;
        }

        audioSource.pitch = attack.parriedPitch;
        if (attack.parriedSound != null)
            audioSource.PlayOneShot(attack.parriedSound);
        else
            audioSource.PlayOneShot(audioSource.clip);
        parryCDend = Time.time - 100 * parryWindow;
        parryStart = Time.time - 100 * parryWindow;
        parryLight.intensity = parriedEnergy * 40;
        // Debug.Log(parryLight.intensity);
        // playerScript.GetComponent<PlayerControlScript>().cameraScript.ScreenShakeFov();
        TryToParryFromBuffer();
    }


    public void ResetBufferAndCd()
    {
        parryBuffer.Clear();
        parryCDend = Time.time - 100 * parryWindow;
        parryStart = Time.time - 100 * parryWindow;
        blockBufferTime = Time.time + 0.1f;
    }

    public void StartBlocking()
    {
        isBlocking = true;
    }

    public void EndBlocking()
    {
        isBlocking = false;
    }

    public void BlockAttack(AttackScript attack)
    {
        float energyAvailable = characterStateScript.BurstEnergy;
        float energyConsume = Mathf.Min(energyAvailable, attack.energy * blockEnergyConsumeMultiplier);
        attack.energy -= energyConsume / blockEnergyConsumeMultiplier;
        characterScript.ConsumeBurstEnergy(energyConsume);

        float BurstEnergyMax = characterStateScript.BurstEnergyMax;
        if (energyConsume <= 0.2f * BurstEnergyMax)
            audioSource.PlayOneShot(BlockSoundLight);
        else if (energyConsume <= 0.4f * BurstEnergyMax)
            audioSource.PlayOneShot(BlockSoundMedium);
        else
            audioSource.PlayOneShot(BlockSoundHeavy);

        if (attack.energy == 0)
            attack.OnParried(characterScript.gameObject);
        else
            audioSource.PlayOneShot(BlockSoundBreak);

        float f = 8 * (1 + energyConsume * 0.02f);
        playerScript.cameraScript.ScreenShakeRotSHM((UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1) * Mathf.Sqrt(energyConsume) * 0.5f, f, 2f, f * Mathf.Lerp(0.05f, 0.12f, energyConsume * 0.02f));


    }
    public bool IsParrying()
    {
        return Time.time <= parryStart + parryWindow;
    }

    public bool IsUsingSword()
    {
        return isBlocking || IsParrying();
    }
}
