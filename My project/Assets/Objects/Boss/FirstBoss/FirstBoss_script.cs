using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstBoss_script : MonoBehaviour
{
    public Object[] attacks;
    public GameObject target;
    public GameObject sword;
    public int testingAttackId = 0;
    public float testingAttackDifficulty = 0.2f;
    [HideInInspector]
    public GameObject currentAttackGenerator = null;
    public float difficulty = 0.2f;

    private Vector3 swordOriginalTransformPosition;
    private Quaternion swordOriginalTransformRotation;
    private Vector3 swordOriginalTransformScale;
    private bool autoAttack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swordOriginalTransformPosition = sword.transform.localPosition;
        swordOriginalTransformRotation = sword.transform.localRotation;
        swordOriginalTransformScale = sword.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        ChooseAnAttack();
    }

    void Movement()
    {
        Vector3 eular = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
        eular.x = 0; eular.z = 0;
        transform.rotation = Quaternion.Euler(eular);

        if (currentAttackGenerator == null)
        {
            // sword.transform.localPosition = swordOriginalTransformPosition;
            // sword.transform.localRotation = swordOriginalTransformRotation;
            // sword.transform.localScale = swordOriginalTransformScale;
        }
    }



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
                RandomPickAttack();
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
}
