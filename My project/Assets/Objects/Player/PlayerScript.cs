using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterMovementScript))]
[RequireComponent(typeof(Animator))]
public class PlayerScript : CharacterScript
{
    public AudioClip DamagedSound;
    AudioSource audioSource;
    public PlayerParryBoxscript parry;
    public GameObject sword;
    public GameObject blade;
    public GameObject attackBox;
    public GameObject[] Attacks;

    GameObject currentAttack;

    Animator animator;



    override protected void Awake()
    {
        base.Awake();
        // Debug.Log("PS cs: " + characterScript);
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        attackBox = transform.Find("PlayerAttackBox").gameObject;
        sword = transform.Find("Sword").gameObject;
        blade = sword.transform.Find("blade").gameObject;
        OnDamageObservers += OnDamage;
    }

    void OnDestroy()
    {
        OnDamageObservers -= OnDamage;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    float animatorStallTime = 0;
    override protected void Update()
    {
        base.Update();
        stateScript.BurstEnergy = Mathf.Min(stateScript.BurstEnergy + 1.5f * Time.deltaTime, stateScript.BurstEnergyMax);
        animator.Update(0);
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.normalizedTime >= 1)
        {
            if (animatorStateInfo.normalizedTime >= 1 && parry.isBlocking && !parry.IsParrying())
            {
                animator.Play("Player Block");
            }
            if (animatorStateInfo.normalizedTime >= 1 && SwordCanParry() && !parry.isBlocking && SwordCanAttack())
            {
                animatorStallTime += Time.deltaTime;
                if (animatorStallTime >= 0.4f)
                    animator.Play("Player Idle");
            }
        }
        else
        {
            animatorStallTime = 0;
        }
    }


    public void NormalSlash()
    {
        if (SwordCanAttack())
        {
            currentAttack = InstantiateAttack(Attacks[0]);
        }
    }

    public void Parry()
    {
        if (SwordCanParry())
        {
            parry.AddParryToBuffer();
            if (!parry.isBlocking)
            {
                animator.Play("Player Parry 1");
            }
            else
            {
                animator.Play("Player Parry 2");
            }
        }
    }

    public void StartBlocking()
    {
        if (SwordCanParry())
        {
            parry.StartBlocking();
        }
    }
    public void EndBlocking()
    {
        parry.EndBlocking();
    }

    public void OnDamage()
    {
        audioSource.PlayOneShot(DamagedSound);
        parry.ResetBufferAndCd();
    }

    public bool SwordCanParry()
    {
        return currentAttack == null;
    }

    public bool SwordCanAttack()
    {
        return !parry.IsUsingSword() && currentAttack == null;
    }


    public GameObject InstantiateAttack(Object gen)
    {
        GameObject genobj = (GameObject)Instantiate(gen);
        AttackGenerator attackGenerator = genobj.GetComponent<AttackGenerator>();
        attackGenerator.spawner = gameObject;
        return genobj;
    }
}
