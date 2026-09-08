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

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canTurn = true;
    [HideInInspector] public bool canAct = true;
    [HideInInspector] public bool defeated = false;

    GameObject currentAttack;

    Animator animator;
    [HideInInspector] public CameraScript cameraScript;



    override protected void Awake()
    {
        base.Awake();
        // Debug.Log("PS cs: " + characterScript);
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        attackBox = transform.Find("PlayerAttackBox").gameObject;
        sword = transform.Find("Sword").gameObject;
        blade = sword.transform.Find("blade").gameObject;
        cameraScript = GetComponent<PlayerControlScript>().playerCamera.GetComponent<CameraScript>();
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

        if (currentAttack == null)
        {
            canMove = true;
            canTurn = true;
        }

        if (stateScript.Hp <= 0 && !defeated)
        {
            Destroy(currentAttack);
            currentAttack = InstantiateAttack(Attacks[3]);
            defeated = true;
        }
    }


    public void NormalSlash()
    {
        if (SwordCanAttack())
        {
            currentAttack = InstantiateAttack(Attacks[0]);
        }
    }

    public void DashSlash()
    {
        if (SwordCanAttack() && Attacks[1].GetComponent<AttackGenerator>().maxEnergyCost <= stateScript.BurstEnergy)
        {
            currentAttack = InstantiateAttack(Attacks[1]);
        }
    }
    public void Heal()
    {
        if (SwordCanAttack() && Attacks[2].GetComponent<AttackGenerator>().maxEnergyCost <= stateScript.BurstEnergy)
        {
            currentAttack = InstantiateAttack(Attacks[2]);
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

    public void OnDamage(float damage)
    {
        audioSource.PlayOneShot(DamagedSound);
        parry.ResetBufferAndCd();
        float f = 8 * (1 + damage * 0.02f);
        cameraScript.ScreenShakeRotSHM((Random.Range(0, 2) == 0 ? -1 : 1) * Mathf.Sqrt(damage) * 0.5f, f, 2f, f * Mathf.Lerp(0.05f, 0.12f, damage * 0.02f));
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
