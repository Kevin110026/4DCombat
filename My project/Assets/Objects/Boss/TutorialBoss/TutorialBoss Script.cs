using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

public class TutorialBossScript : CharacterScript
{
    public Object[] attacks;
    public Object[] movements;
    public GameObject target;
    public GameObject sword;
    public GameObject blade;
    public int testingAttackId = 0;
    public float testingAttackDifficulty = 0.2f;
    [HideInInspector]
    public GameObject currentAttackGenerator = null;
    public GameObject currentMovementGenerator = null;
    public float difficulty = 0.2f;
    public AudioClip DamagedSound;
    [SerializeField] float RandomAttackCD;
    private bool autoAttack = false;
    private AudioSource audioSource;

    Animator animator;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        audioSource = body.GetComponent<AudioSource>();
        sword = transform.Find("Sword").gameObject;
        blade = sword.transform.Find("blade").gameObject;
        OnDamageObservers += OnDamage;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        Movement();
        EnergyRecovery();
        ChooseAnAttack();
    }

    void Movement()
    {
        if (currentAttackGenerator == null)
        {
            if (currentMovementGenerator == null)
                currentMovementGenerator = InstantiateAttack(movements[0]);
        }
        else
        {
            Destroy(currentMovementGenerator);
        }
    }

    public void FaceTarget()
    {
        Vector3 eular = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
        eular.x = 0; eular.z = 0;
        transform.rotation = Quaternion.Euler(eular);

    }

    void EnergyRecovery()
    {
        float recovery = attackActive == true ? 0 * Time.deltaTime : 16f * Time.deltaTime;
        stateScript.BurstEnergy = Mathf.Min(stateScript.BurstEnergy + recovery, stateScript.BurstEnergyMax);
    }



    float Random_RandomAttackCD = 0;
    float attackCDTimeLen = 0;
    int attackEnergyFailCount = 0;
    bool attackActive = true;
    void ChooseAnAttack()
    {
        {   //  testing-code
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                int i = testingAttackId;
                Object attack = attacks[i];
                Object instant = Instantiate(attack, new Vector3(0, 0, 0), new Quaternion());
                currentAttackGenerator = (GameObject)instant;
                AttackGenerator attack_generator_script = instant.GetComponent<AttackGenerator>();
                if (attack_generator_script == null)
                {
                    Debug.Log("error! generated none-attack-generator!");
                    return;
                }
                attack_generator_script.spawner = gameObject;
                attack_generator_script.target = target;
                attack_generator_script.difficulty = testingAttackDifficulty;

            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RandomPickAttack();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                autoAttack = !autoAttack;
            }
        }



        if (!attackActive)
        {
            float threshold = 40f;
            if (stateScript.BurstEnergy >= threshold)
            {
                if (Random.Range(0f, 1f) < (stateScript.BurstEnergy - threshold) / (stateScript.BurstEnergyMax - threshold) * Time.deltaTime * 2)
                {
                    attackActive = true;
                    attackEnergyFailCount = 0;
                }
            }
        }

        if (attackActive)
        {
            if (autoAttack)
            {
                if (currentAttackGenerator == null)
                {
                    attackCDTimeLen += Time.deltaTime;
                    if (attackCDTimeLen >= Random_RandomAttackCD)
                    {
                        if (RandomPickAttack())
                        {
                            attackCDTimeLen = 0;
                            Random_RandomAttackCD = Random.Range(0.25f, 0.35f);
                        }
                        else
                        {
                            attackEnergyFailCount++;
                            if (attackEnergyFailCount >= 4)
                            {
                                attackActive = false;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("active:" + attackActive + " energy: " + stateScript.BurstEnergy);
    }

    bool RandomPickAttack()
    {
        int i = Random.Range(0, attacks.Length);
        Object attack = attacks[i];
        if (attack.GetComponent<AttackGenerator>().maxEnergyCost <= stateScript.BurstEnergy)
        {
            currentAttackGenerator = InstantiateAttack(attack);
            return true;
        }
        else
        {
            return false;
        }
    }

    GameObject InstantiateAttack(Object attack)
    {
        Object instant = Instantiate(attack, new Vector3(0, 0, 0), new Quaternion());
        AttackGenerator attack_generator_script = instant.GetComponent<AttackGenerator>();
        if (attack_generator_script == null)
        {
            Debug.Log("error! generated none-attack-generator!");
            return null;
        }
        attack_generator_script.spawner = this.gameObject;
        attack_generator_script.target = target;
        attack_generator_script.difficulty = difficulty;
        return (GameObject)instant;
    }

    public void OnDamage()
    {
        audioSource.PlayOneShot(DamagedSound);
        if (currentAttackGenerator == null)
        {
            currentAttackGenerator = InstantiateAttack(movements[1]);
        }
    }
    void OnDestroy()
    {
        OnDamageObservers -= OnDamage;
    }
}
