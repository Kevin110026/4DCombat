using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

public class TutorialBossScript : CharacterScript
{
    public Object[] attacks;
    public GameObject target;
    public GameObject sword;
    public GameObject blade;
    public int testingAttackId = 0;
    public float testingAttackDifficulty = 0.2f;
    [HideInInspector]
    public GameObject currentAttackGenerator = null;
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
        ChooseAnAttack();
    }

    void Movement()
    {
        if (currentAttackGenerator == null)
        {
            FaceTarget();
            animator.Play("TutorialBoss Idle");
        }
    }

    public void FaceTarget()
    {
        Vector3 eular = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
        eular.x = 0; eular.z = 0;
        transform.rotation = Quaternion.Euler(eular);

    }



    float Random_RandomAttackCD = 0;
    float attackCDTimeLen = 0;
    void ChooseAnAttack()
    {
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
        if (autoAttack)
        {
            if (currentAttackGenerator == null)
            {
                attackCDTimeLen += Time.deltaTime;
                if (attackCDTimeLen >= Random_RandomAttackCD)
                {
                    attackCDTimeLen = 0;
                    Random_RandomAttackCD = Random.Range(0.35f, 0.65f);
                    RandomPickAttack();
                }
            }
        }
    }

    void RandomPickAttack()
    {
        int i = Random.Range(0, attacks.Length);
        Object attack = attacks[i];
        Object instant = Instantiate(attack, new Vector3(0, 0, 0), new Quaternion());
        currentAttackGenerator = (GameObject)instant;
        AttackGenerator attack_generator_script = instant.GetComponent<AttackGenerator>();
        if (attack_generator_script == null)
        {
            Debug.Log("error! generated none-attack-generator!");
            return;
        }
        attack_generator_script.spawner = this.gameObject;
        attack_generator_script.target = target;
        attack_generator_script.difficulty = difficulty;
    }


    public void OnDamage()
    {
        audioSource.PlayOneShot(DamagedSound);
    }
    void OnDestroy()
    {
        OnDamageObservers -= OnDamage;
    }
}
