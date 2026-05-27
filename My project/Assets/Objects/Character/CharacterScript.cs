using UnityEngine;

[RequireComponent(typeof(CharacterMovementScript))]
[RequireComponent(typeof(CharacterHpScript))]
[RequireComponent(typeof(CharacterEnergyScript))]

public class CharacterScript : MonoBehaviour
{
    [HideInInspector] public CharacterMovementScript movementScript;
    [HideInInspector] public CharacterHpScript hpScript;
    [HideInInspector] public CharacterEnergyScript energyScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movementScript = GetComponent<CharacterMovementScript>();
        hpScript = GetComponent<CharacterHpScript>();
        energyScript = GetComponent<CharacterEnergyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
