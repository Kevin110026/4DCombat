using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public bool canDamage = true;
    public float energy = 0;
    public float damage = 0;
    public bool haveDamaged = false;
    public bool canBeParried = true;
    public float parriedPitch = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip parriedSound;
    [HideInInspector] public GameObject spawner;
    [HideInInspector] public GameObject target;
    [HideInInspector] public AttackGenerator attackGenerator;

    [HideInInspector] public Vector3 knockbackDir = Vector3.zero;
    public float knockbackMag = 0;
    [HideInInspector] public bool dontDestroyGameobject = false;

    // public bool isSnapShot = false;

    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }


    //  wait for the parry after the hit
    [HideInInspector] public GameObject TheHitted;
    virtual public void OnHitCharacter(GameObject character)
    {
        // Debug.Log("hit character");
        if (TheHitted != null)
            return;
        // Debug.Log("set the hitted = " + character);
        TheHitted = character;
        if (!hittedByParryBox)
            DamageCharacter(character);
    }

    [HideInInspector] public bool hittedByParryBox = false;
    // LinkedListNode<Tuple<AttackScript, float>> hitParryBoxRecord;
    virtual public void OnHitParryBox(PlayerParryBoxscript parryBox)
    {
        if (!canBeParried || hittedByParryBox)
            return;
        // hitParryBoxRecord =
        hittedByParryBox = true;
        parryBox.RegisterAttack(this);
        CharacterController characterController = parryBox.playerScript.GetComponent<CharacterController>();
        characterController.Move(0.3f * knockbackMag * knockbackDir.normalized);
    }

    public void ContinueDamageCharacter()
    {
        // Debug.Log("continue");
        hittedByParryBox = false;
        if (TheHitted != null)
            DamageCharacter(TheHitted);
        // else
        //     Debug.Log("null");
        if (scheduledToDestroy)
            this.Destroy();
    }

    virtual protected void DamageCharacter(GameObject character)
    {
        if (haveDamaged || !canDamage)
            return;
        CharacterScript characterScript = character.GetComponent<CharacterScript>();
        characterScript.Damage(damage);
        haveDamaged = true;
        canBeParried = false;
        canDamage = false;

        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.Move(0.7f * knockbackMag * knockbackDir.normalized);
        if (!hittedByParryBox)
            characterController.Move(0.3f * knockbackMag * knockbackDir.normalized);
        // Debug.Log(knockbackMag * knockbackDir);
    }
    public virtual void OnParried(GameObject parrier)
    {
        canDamage = false;
        canBeParried = false;

        // CharacterController characterController = parrier.GetComponent<CharacterController>();
        // characterController.Move(0.3f * knockbackMag * knockbackDir.normalized);
        attackGenerator.OnParried(parrier);
    }

    bool scheduledToDestroy = false;
    public virtual void ScheduleDestroy()
    {
        if (this == null)
            return;
        scheduledToDestroy = true;
        this.enabled = false;
        if (!hittedByParryBox)
            this.Destroy();
    }
    public virtual void Destroy()
    {
        if (dontDestroyGameobject)
            Destroy(this);
        else
            Destroy(gameObject);
    }

    // protected virtual void OnDestroy()
    // {
    //     if (isSnapShot)
    //         return;
    //     if (hitParryBoxRecord == null)
    //         return;


    // }
}
