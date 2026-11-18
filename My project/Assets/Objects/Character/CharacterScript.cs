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

    public float Damage(float damage)
    {
        float ret = stateScript.Damage(damage);
        OnDamageObservers?.Invoke(damage);
        return ret;
    }

    public event Action<float> OnHealObservers;

    public float Heal(float heal)
    {
        float ret = stateScript.Heal(heal);
        OnHealObservers?.Invoke(heal);
        return ret;
    }
    public event Action OnConsumeBurstEnergyObservers;

    public float ConsumeBurstEnergy(float energy)
    {
        float ret = stateScript.ConsumeBurstEnergy(energy);
        OnConsumeBurstEnergyObservers?.Invoke();
        return ret;
    }
    public event Action OnConsumeTotalEnergyObservers;

    public float ConsumeTotalEnergy(float energy)
    {
        float ret = stateScript.ConsumeTotalEnergy(energy);
        OnConsumeTotalEnergyObservers?.Invoke();
        return ret;
    }

}
