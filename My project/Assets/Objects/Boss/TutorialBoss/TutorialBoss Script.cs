using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore;

[RequireComponent(typeof(Animator))]
public class TutorialBossScript : CharacterScript
{
    public Object[] attacks;
    public int[] attacksWeights = { };
    public Object[] movements;
    public Object KnockedAnim;
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
    [SerializeField] public bool autoAttack = false;
    private AudioSource audioSource;
    [SerializeField] int stage = 0;
    private int[] stageAttackCount = { 5, 11, 999 };
    [SerializeField] int[] stageHp = { 200, 300, 600 };
    [SerializeField] int[] stageEnergyMax = { 20, 50, 70 };
    [SerializeField] int[] stageAttackActive = { 18, 40, 60 };
    [SerializeField] int[] stageEnergyRecovery = { 6, 16, 28 };
    private bool knocked = false;
    private bool fighting = true;
    [HideInInspector] public bool start = false;

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
        ResetState();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        if (!start)
            return;
        fighting = target.GetComponent<CharacterStateScript>().Hp > 0;
        if (!fighting)
        {
            Destroy(currentAttackGenerator);
            FaceTarget();
            return;
        }

        if (!autoAttack && (transform.position - target.transform.position).magnitude <= 6)
            autoAttack = true;


        Movement();
        EnergyRecovery();
        ChooseAnAttack();

        if (lastHpCd < Time.time || attackActive)
        {
            lastHp = Mathf.Max(lastHp - Time.deltaTime * 20, stateScript.Hp);
        }
        if (!knocked && stateScript.Hp <= 0)
        {
            Destroy(currentAttackGenerator);
            currentAttackGenerator = InstantiateAttack(KnockedAnim);
            knocked = true;
            if (stage < 2)
            {
                Camera.allCameras[0].GetComponentInParent<CameraScript>().ScreenShakeFov(-10f, 0.66f, 0.34f);
            }
            else
            {
                Camera.allCameras[0].GetComponentInParent<CameraScript>().ScreenShakeFov(-20f, 1.3f, 0.5f);
                Camera.allCameras[0].GetComponentInParent<CameraScript>().ScreenShakeFovSHM(2, 12, 1, 4);
            }
        }
        if (knocked && currentAttackGenerator == null)
        {
            if (stage < 3)
            {
                stage++;
            }
            if (stage < 3)
            {
                ResetState();
                knocked = false;
                attackActive = true;
            }
            else
            {
                knocked = true;
                attackActive = false;
            }
        }
    }

    void Movement()
    {
        if (currentAttackGenerator == null && !knocked)
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
        float recoveryRate = stage < 3 ? stageEnergyRecovery[stage] : 0;
        float recovery = attackActive == true ? 0 * Time.deltaTime : recoveryRate * Time.deltaTime;
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
                currentAttackGenerator = InstantiateAttack(attack);
                // Object instant = Instantiate(attack, new Vector3(0, 0, 0), new Quaternion());
                // currentAttackGenerator = (GameObject)instant;
                // AttackGenerator attack_generator_script = instant.GetComponent<AttackGenerator>();
                // if (attack_generator_script == null)
                // {
                //     Debug.Log("error! generated none-attack-generator!");
                //     return;
                // }
                // attack_generator_script.spawner = gameObject;
                // attack_generator_script.target = target;
                // attack_generator_script.difficulty = testingAttackDifficulty;

            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RandomPickAttack();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                autoAttack = !autoAttack;
                start = !start;
            }
        }



        if (!attackActive && !knocked)
        {
            float threshold = stageAttackActive[stage];
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

        // Debug.Log("active:" + attackActive + " energy: " + stateScript.BurstEnergy);
    }

    bool RandomPickAttack()
    {
        int size = Mathf.Min(stageAttackCount[stage], attacks.Length);
        int i = WeightedRNG.Get(attacksWeights.Take(size).ToArray());
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
        AttackGenerator[] attack_generator_scripts = instant.GetComponents<AttackGenerator>();
        // if (attack_generator_script == null)
        // {
        //     Debug.Log("error! generated none-attack-generator!");
        //     return null;
        // }
        foreach (AttackGenerator attGenScr in attack_generator_scripts)
        {
            attGenScr.spawner = this.gameObject;
            attGenScr.target = target;
            attGenScr.difficulty = difficulty;
        }
        return (GameObject)instant;
    }

    float lastHp;
    float lastHpCd = 0;
    float bigBackOffCd = 0;
    public void OnDamage(float damage)
    {
        lastHpCd = Time.time + 2;
        audioSource.PlayOneShot(DamagedSound);
        // Debug.Log(lastHp - stateScript.Hp);
        if (currentAttackGenerator == null)
        {
            if (stage == 3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            currentAttackGenerator = InstantiateAttack(movements[1]);
            if (lastHp - stateScript.Hp >= 30 && Time.time > bigBackOffCd)
            {
                // Debug.Log("BIG");
                currentAttackGenerator.GetComponent<TutorialBossMovementBackOff>().BackOffDistance = 15f;
                // currentAttackGenerator.GetComponent<TutorialBossMovementBackOff>().BackOffAcceleration *= 3f;
                bigBackOffCd = Time.time + 5f;
            }
        }
    }

    public void ResetState()
    {
        stateScript.HpMax = stageHp[stage];
        stateScript.Hp = stateScript.HpMax;
        stateScript.BurstEnergyMax = stageEnergyMax[stage];
        stateScript.BurstEnergy = stateScript.BurstEnergyMax;
        lastHp = stateScript.Hp;
    }
    void OnDestroy()
    {
        OnDamageObservers -= OnDamage;
    }

}
