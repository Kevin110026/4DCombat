using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ViperStrikeattackgeneratorscript : AttackGenerator
{
    public UnityEngine.Object StrikePerfab;
    private CharacterMovementScript movement;

    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        movement = spawner.GetComponent<CharacterMovementScript>();
        movement.maxMoveSpeed = 50;
        movement.maxAcceleration = 300;
        whosh = Resources.Load<AudioClip>("Low Whoosh");
        slash = Resources.Load<AudioClip>("parry-3-limbus");
        hint = Resources.Load<AudioClip>("A Elec Guitar");
        audioSource = spawner.GetComponent<AudioSource>();
        // difficulity = 1.0f;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator StartAttack()
    {
        float delay = Mathf.Lerp(0.4f, 0.2f, difficulty);
        int cycles = Mathf.RoundToInt(UnityEngine.Random.Range(1 + difficulty * 2f, 2 + difficulty * 2.49f));
        bool gottenClose = false;
        while (!gottenClose)
        {
            gottenClose = GetCloseToTarget();
            yield return null;
        }
        yield return new WaitForSeconds(Mathf.Lerp(0.55f, 0.4f, difficulty) - delay);
        for (int i = 0; i < cycles; i++)
        {
            int dashes = UnityEngine.Random.Range(1, 4);
            List<float> dashTimeStamps = new List<float>(dashes);
            float t = 0.0f;
            float delayRngRange = Mathf.Lerp(0.1f, 0.5f, difficulty);
            for (int j = 0; j < dashes; j++)
            {
                dashTimeStamps.Add(t + delay * UnityEngine.Random.Range(1 - delayRngRange, 1 + delayRngRange));
                t = dashTimeStamps[j];
                StartCoroutine(DashTowardTargetCoroutine(dashTimeStamps[j]));
            }
            float slashTime = t + delay;
            yield return new WaitForSeconds(slashTime - 2 * delay);
            playHint(delay);
            yield return new WaitForSeconds(2 * delay);

            Slash(i == cycles - 1);
            yield return new WaitForSeconds(Mathf.Lerp(0.8f, 0.4f, difficulty) - delay);
        }

        Destroy(this.gameObject);
    }

    bool GetCloseToTarget()
    {
        // movement.MoveToXZ(target.transform.position, -1, target.GetComponent<CharacterMovementScript>().velocity);
        Vector3 deltaPos = spawner.transform.position - target.transform.position;
        deltaPos = deltaPos.normalized * 3f;
        movement.MoveToXZ(target.transform.position + deltaPos);
        if ((target.transform.position - spawner.transform.position).magnitude <= 4f)
        {
            movement.DoneMoveTo();
            return true;
        }
        else
        {
            return false;
        }
    }

    private const float spinDegRange = 60f;

    IEnumerator DashTowardTargetCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DashTowardTarget();
    }
    void DashTowardTarget()
    {
        Vector3 deltaPos = spawner.transform.position - target.transform.position;
        deltaPos = deltaPos.normalized;
        float pow = 4f;
        float spinDeg = Mathf.Pow(UnityEngine.Random.Range(0, Mathf.Pow(spinDegRange, pow)), 1 / pow);
        spinDeg *= UnityEngine.Random.Range(0, 2) * 2 - 1;  //  +- 1
        Vector3 finalDeltaPos = Quaternion.Euler(0, spinDeg, 0) * deltaPos * 2f;
        movement.maxAcceleration = 1200;
        movement.maxMoveSpeed = 600;
        movement.MoveToXZ(target.transform.position + finalDeltaPos);
        audioSource.PlayOneShot(whosh);
    }
    private AudioClip whosh;
    private AudioClip slash;
    private AudioClip hint;

    void Slash(bool finalShash = false)
    {

        Vector3 deltaPos = spawner.transform.position - target.transform.position;
        deltaPos = deltaPos.normalized;
        Vector3 finalDeltaPos = deltaPos * 2f;
        movement.maxAcceleration = 1200;
        movement.maxMoveSpeed = 600;
        movement.MoveToXZ(target.transform.position + finalDeltaPos);
        audioSource.PlayOneShot(whosh);

        // audioSource.PlayOneShot(slash);
        GameObject strike = (GameObject)Instantiate(StrikePerfab, spawner.transform.position, Quaternion.LookRotation(target.transform.position - spawner.transform.position));
        AttackScript ats = strike.GetComponent<AttackScript>();
        if (!finalShash)
            ats.parriedSound = Resources.Load<AudioClip>("parry-1-limbus");
        else
            ats.parriedSound = Resources.Load<AudioClip>("parry-3-limbus");
    }

    void playHint(float delay)
    {
        audioSource.clip = hint;
        audioSource.time = hint.length - delay;
        audioSource.Play();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        movement.ResetMaxSpeedAndAcceletarion();

    }
}
