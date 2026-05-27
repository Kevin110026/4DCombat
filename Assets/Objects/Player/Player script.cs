using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterMovementScript))]

public class Playerscript : MonoBehaviour
{
    public AudioClip DamagedSound;
    public float hp = 100.0f;
    AudioSource audioSource;

    public PlayerParryBoxscript parry;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && SwordCanParry())
        {
            parry.AddParryToBuffer();
        }
    }

    public void Damaged(float damage)
    {
        hp -= damage;
        audioSource.PlayOneShot(DamagedSound);
        parry.ResetBufferAndCd();
    }

    bool SwordCanParry()
    {
        return true;
    }

    bool SwordCanAttack()
    {
        return !parry.IsParrying();
    }
}
