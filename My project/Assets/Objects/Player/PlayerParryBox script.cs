using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public float parryWindow = 0.1f;
    public float imperfectParryWindow = 0.15f;
    public float clearMissTime = 0.35f;
    public float blockEnergyConsumeMultiplier = 2;
    public GameObject parryLightObject;
    private Light parryLight;
    public AudioClip[] parrySounds;
    private LinkedList<Tuple<float, float>> parryBuffer;
    private LinkedList<Tuple<AttackScript, float>> attackBuffer;
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
    public AudioClip HitParryBoxSound;


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
        audioSource = GetComponent<AudioSource>();
        parryLight = parryLightObject.GetComponent<Light>();
        parryBuffer = new LinkedList<Tuple<float, float>>();
        attackBuffer = new LinkedList<Tuple<AttackScript, float>>();
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
        CleanBuffers();
        if (Time.time >= parryMissLastTime + clearMissTime)
            parryMissCount = 0;
        // while (parryBuffer.Count > 0 && Time.time > parryBuffer.First.Value + parryWindow)
        //     parryBuffer.RemoveFirst();
        // TryToParryFromBuffer();
    }

    // bool TryToParryFromBuffer()
    // {
    //     if (parryBuffer.Count > 0 && TryToParry(parryBuffer.First.Value))
    //     {
    //         parryBuffer.RemoveFirst();
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // bool TryToParry(float parryTime)
    // {
    //     if (parryTime >= parryCDend && characterStateScript.BurstEnergy >= parryEnergyConsume)
    //     {
    //         parryStart = parryTime;
    //         parryCDend = parryStart + parryWindow + parryCD;
    //         characterScript.ConsumeBurstEnergy(parryEnergyConsume);
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }



    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AttackScript>(out AttackScript attack))
        {
            attack.OnHitParryBox(this);
            // attackBuffer.AddLast(new Tuple<AttackScript, float>(attack, Time.time));
            // if (attack.canBeParried && !attack.hittedByParryBox)
        }
    }

    float lastParryTime = 0;
    public void RegisterParry()
    {
        if (Time.time >= lastParryTime + imperfectParryWindow / 2)
        {
            float baseParryEnergy = Mathf.Min(playerScript.stateScript.BurstEnergy, parryEnergyConsume);
            playerScript.ConsumeBurstEnergy(baseParryEnergy);
            parryBuffer.AddLast(new Tuple<float, float>(baseParryEnergy, Time.time));
            lastParryTime = Time.time;
            MatchParryAndAttack();
            if (parryBuffer.Count > 0)
            {
                if (playerScript.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Player Parry 1" ||
                playerScript.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    playerScript.animator.Play("Player Parry 1", -1, 0);
                    // playerScript.animator.GetCurrentAnimatorClipInfo(0)[0].clip.time = 0;
                }
            }
        }
    }
    public LinkedListNode<Tuple<AttackScript, float>> RegisterAttack(AttackScript attack)
    {
        LinkedListNode<Tuple<AttackScript, float>> ret = attackBuffer.AddLast(new Tuple<AttackScript, float>(attack, Time.time));

        // audioSource.PlayOneShot(HitParryBoxSound);

        MatchParryAndAttack();
        if (attackBuffer.Count > 0)
        {
            if (playerScript.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Player Block" ||
                playerScript.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                playerScript.animator.Play("Player Block", -1, 0);
            }
        }
        return ret;
    }

    // public void VerifyParryForAttack(AttackScript attack)
    // {
    //     if (attack.canBeParried)
    //     {
    //         if (Time.time <= parryStart + parryWindow)
    //         {
    //             OnParry(attack);
    //         }
    //         else if (isBlocking)
    //         {
    //             BlockAttack(attack);
    //         }
    //     }
    // }

    int parryMissCount = 0;
    float parryMissLastTime = 0;
    void MissParry(float parryTime)
    {
        parryMissCount = Math.Min(2, parryMissCount + 1);
        parryMissLastTime = parryTime;
    }
    public void MatchParryAndAttack()
    {
        CleanBuffers();
        while (parryBuffer.Count > 0 && attackBuffer.Count > 0)
        {
            OnParry(attackBuffer.First.Value.Item1, attackBuffer.First.Value.Item2, parryBuffer.First.Value.Item1, parryBuffer.First.Value.Item2);
            parryBuffer.RemoveFirst();
            attackBuffer.RemoveFirst();
        }
    }

    void CleanParryBuffer()
    {
        while (parryBuffer.Count > 0 && Time.time - parryBuffer.First.Value.Item2 > imperfectParryWindow / 2)
        {
            MissParry(parryBuffer.First.Value.Item2);
            parryBuffer.RemoveFirst();
        }
    }

    void CleanAttackBuffer()
    {
        while (attackBuffer.Count > 0 && Time.time - attackBuffer.First.Value.Item2 > imperfectParryWindow / 2)
        {
            if (isBlocking)
                BlockAttack(attackBuffer.First.Value.Item1);
            attackBuffer.First.Value.Item1.ContinueDamageCharacter();
            attackBuffer.RemoveFirst();
        }
    }

    void CleanBuffers()
    {
        CleanParryBuffer();
        CleanAttackBuffer();
    }

    void OnParry(AttackScript attack, float hitTime, float baseParryEnergy, float parryTime)
    {
        float energyCostMultplier = 1;
        bool imperfect = Mathf.Abs(hitTime - parryTime) >= parryWindow / 2;
        // Debug.Log(hitTime - parryTime);
        if (!imperfect)
        {
            if (parryMissCount == 0)
                energyCostMultplier = 0.2f;
            else if (parryMissCount == 1)
                energyCostMultplier = 0.6f;
            else if (parryMissCount == 2)
                energyCostMultplier = 1.0f;
            else if (parryMissCount >= 3)
                energyCostMultplier = 1.2f;
            parryMissCount = Mathf.Max(0, parryMissCount - 1);
        }
        else
        {
            if (parryMissCount == 0)
                energyCostMultplier = 0.4f;
            else if (parryMissCount == 1)
                energyCostMultplier = 0.8f;
            else if (parryMissCount == 2)
                energyCostMultplier = 1.2f;
            else if (parryMissCount >= 3)
                energyCostMultplier = 1.6f;
        }

        // Debug.Log("player energy: " + playerScript.stateScript.BurstEnergy);
        float energyConsume = Mathf.Min(attack.energy * energyCostMultplier, playerScript.stateScript.BurstEnergy + baseParryEnergy);
        // Debug.Log("consume: " + energyConsume);
        float parriedEnergy = energyConsume / energyCostMultplier;
        attack.energy -= parriedEnergy;
        // Debug.Log(playerScript.stateScript.BurstEnergy + " - " + Mathf.Max(0, energyConsume - baseParryEnergy));
        playerScript.ConsumeBurstEnergy(Mathf.Max(0, energyConsume - baseParryEnergy));

        // old calculation
        // float parryEnergy = parryEnergyConsume * Mathf.Lerp(7f, 2.5f, (parryTime - parryStart) / parryWindow);
        // float parriedEnergy = Mathf.Min(attack.energy, parryEnergy);
        // attack.energy -= parriedEnergy;
        // characterScript.stateScript.BurstEnergy = Mathf.Min(characterScript.stateScript.BurstEnergy + parryEnergyConsume * 0.5f, characterScript.stateScript.BurstEnergyMax);
        // // characterScript.ConsumeBurstEnergy(-parryEnergyConsume * 0.5f);

        // Debug.Log("mult: " + energyCostMultplier);
        // Debug.Log("att ene: " + attack.energy);
        if (attack.energy <= 0.0001f)
        {
            attack.OnParried(characterScript.gameObject);
        }
        else
        {
            BlockAttack(attack);
        }

        if (playerScript.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player Block")
        {
            playerScript.animator.Play("Player Parry Late", -1, 0);
        }

        bool early = playerScript.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Player Parry 1";
        float pitch = early ? 0.95f : 1.05f;
        audioSource.pitch = attack.parriedPitch;
        AudioClip clip = attack.parriedSound != null ? attack.parriedSound : audioSource.clip;
        AudioPlayer.PlayAudio(clip, pitch, 0f).transform.SetParent(gameObject.transform);
        parryLight.color = !imperfect ? Color.white : Color.orange;
        parryCDend = Time.time - 100 * parryWindow;
        parryStart = Time.time - 100 * parryWindow;
        parryLight.intensity = parriedEnergy * 40;
    }


    // public void ResetBufferAndCd()
    // {
    //     parryBuffer.Clear();
    //     parryCDend = Time.time - 100 * parryWindow;
    //     parryStart = Time.time - 100 * parryWindow;
    //     blockBufferTime = Time.time + 0.1f;
    // }

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
        // Debug.Log("att energy: " + attack.energy);
        float energyAvailable = characterStateScript.BurstEnergy;
        // Debug.Log("player energy: " + energyAvailable);
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

        // Debug.Log("block att ene: " + attack.energy);
        if (attack.energy <= 0.0001f)
        {
            attack.OnParried(characterScript.gameObject);
        }
        else
        {
            audioSource.PlayOneShot(BlockSoundBreak);
            attack.ContinueDamageCharacter();
        }

        float f = 8 * (1 + energyConsume * 0.02f);
        // Debug.Log("consume: " + energyConsume);
        playerScript.cameraScript.ScreenShakeRotSHM((UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1) * Mathf.Sqrt(energyConsume) * 0.5f, f, 2f, f * Mathf.Lerp(0.05f, 0.12f, energyConsume * 0.02f));


    }
    public bool IsParrying()
    {
        return parryBuffer.Count > 0;
    }

    public bool IsUsingSword()
    {
        return isBlocking || IsParrying();
    }
}
