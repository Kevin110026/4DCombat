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
    [HideInInspector] public AttackGenerator AttackGenerator;

    [HideInInspector] public Vector3 knockbackDir = Vector3.zero;
    public float knockbackMag = 0;

    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }
    virtual protected void DamageCharacter(GameObject character)
    {
        if (haveDamaged)
            return;
        CharacterScript characterScript = character.GetComponent<CharacterScript>();
        characterScript.Damage(damage);
        haveDamaged = true;
        canBeParried = false;
        canDamage = false;

        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.Move(knockbackMag * knockbackDir.normalized);
        // Debug.Log(knockbackMag * knockbackDir);
    }
    public virtual void OnParried(GameObject parrier)
    {
        canDamage = false;
        canBeParried = false;

        CharacterController characterController = parrier.GetComponent<CharacterController>();
        characterController.Move(0.3f * knockbackMag * knockbackDir.normalized);
    }
}
