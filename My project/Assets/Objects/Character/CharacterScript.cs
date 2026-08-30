using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementScript))]
[RequireComponent(typeof(CharacterStateScript))]

public class CharacterScript : MonoBehaviour
{
    [HideInInspector] public CharacterMovementScript movementScript { get; private set; }
    [HideInInspector] public CharacterStateScript stateScript { get; private set; }
    protected GameObject body;
    private Material bodyMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        movementScript = GetComponent<CharacterMovementScript>();
        stateScript = GetComponent<CharacterStateScript>();
        stateScript.Hp = stateScript.HpMax;
        stateScript.TotalEnergy = stateScript.TotalEnergyMax;
        stateScript.BurstEnergy = stateScript.BurstEnergyMax;
        lastDamageTime = Time.time - 1000f;
        body = transform.Find("Body").gameObject;
        // Debug.Log(gameObject + ": " + body);
        bodyMaterial = body.GetComponent<MeshRenderer>().material;
        // Debug.Log(bodyMaterial);
    }

    protected virtual void Update()
    {
        if (Time.time - lastDamageTime >= 0.15f)
        {
            body.GetComponent<MeshRenderer>().material = bodyMaterial;
        }
        else
        {
            body.GetComponent<MeshRenderer>().material = Resources.Load<Material>("White");
            // Debug.Log("" + gameObject + body.GetComponent<MeshRenderer>().material);
            // Debug.Log(body);
        }
    }




    public event Action<float> OnDamageObservers;

    float lastDamageTime;

    public void Damage(float damage)
    {
        stateScript.Hp -= damage;
        lastDamageTime = Time.time;
        OnDamageObservers?.Invoke(damage);
    }

    public event Action<float> OnHealObservers;

    public void Heal(float heal)
    {
        stateScript.Hp += heal;
        OnHealObservers?.Invoke(heal);
    }
    public event Action OnConsumeBurstEnergyObservers;

    public void ConsumeBurstEnergy(float energy)
    {
        stateScript.BurstEnergy -= energy;
        OnConsumeBurstEnergyObservers?.Invoke();
    }
    public event Action OnConsumeTotalEnergyObservers;

    public void ConsumeTotalEnergy(float energy)
    {
        stateScript.TotalEnergy -= energy;
        OnConsumeTotalEnergyObservers?.Invoke();
    }

}
