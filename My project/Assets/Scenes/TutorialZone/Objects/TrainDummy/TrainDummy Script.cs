using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrainDummyScript : CharacterScript
{
    [SerializeField] AudioClip DamagedSound;
    [SerializeField] GameObject AttackPrefab;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public GameObject sword;
    [HideInInspector] public GameObject blade;
    GameObject attObj;
    [HideInInspector] public bool active = true;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        audioSource = body.GetComponent<AudioSource>();
        sword = transform.Find("Sword").gameObject;
        blade = sword.transform.Find("blade").gameObject;
        OnDamageObservers += OnDamage;
    }

    protected virtual void Start()
    {
        lastHp = stateScript.Hp;
    }

    protected override void Update()
    {
        base.Update();
        if (!active)
            return;
        if (attObj == null)
        {
            attObj = InstantiateAttack(AttackPrefab);
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
        attack_generator_script.spawner = gameObject;
        return (GameObject)instant;
    }

    float lastHp;
    public void OnDamage(float damage)
    {
        audioSource.PlayOneShot(DamagedSound);
        GameObject fct = FloatingCombatText.NewFCT(damage.ToString());
        Vector3 position = transform.position;
        position.y += 1;
        fct.transform.position = position;
    }

    void OnDestroy()
    {
        OnDamageObservers -= OnDamage;
    }
}
