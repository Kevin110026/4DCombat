using UnityEngine;

public class CharacterStateScript : MonoBehaviour
{
    public float HpMax;
    public float Hp;
    public float TotalEnergyMax;
    public float TotalEnergy;
    public float TotalEnergyRecoverCD = 2;
    public float TotalEnergyRecoverRate;

    public float BurstEnergyMax;
    public float BurstEnergy;
    public float BurstEnergyTransformCD = 2;
    public float BurstEnergyTransformRate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= lastConsumeBurstEnergyTime + BurstEnergyTransformCD)
        {
            float add = Mathf.Min(BurstEnergyMax - BurstEnergy, BurstEnergyTransformRate * Time.deltaTime);
            add = Mathf.Min(add, TotalEnergy);
            BurstEnergy += add;
            TotalEnergy -= add;
        }
        if (Time.time >= lastConsumeTotalEnergyTime + TotalEnergyRecoverCD && Time.time >= lastConsumeBurstEnergyTime + BurstEnergyTransformCD)
        {
            TotalEnergy = Mathf.Min(TotalEnergy + TotalEnergyRecoverRate * Time.deltaTime, TotalEnergyMax);
        }
    }


    public float lastDamageTime = 0;

    public float Damage(float damage)
    {
        Hp -= damage;
        lastDamageTime = Time.time;
        return damage;
    }


    public float Heal(float heal)
    {
        Hp += heal;
        return Hp;
    }

    float lastConsumeBurstEnergyTime = 0;
    public float ConsumeBurstEnergy(float energy)
    {
        float consume = Mathf.Min(BurstEnergy, energy);
        BurstEnergy -= consume;
        if (BurstEnergy <= 0.0001f)
            BurstEnergy = 0;
        ResetBurstEnergyCD();
        return consume;
    }
    public void ResetBurstEnergyCD()
    {
        lastConsumeBurstEnergyTime = Time.time;
    }

    float lastConsumeTotalEnergyTime = 0;
    public float ConsumeTotalEnergy(float energy)
    {
        float consume = Mathf.Min(TotalEnergy, energy);
        TotalEnergy -= consume;
        if (TotalEnergy <= 0.0001f)
            TotalEnergy = 0;
        ResetTotalEnergyCD();
        return consume;
    }
    public void ResetTotalEnergyCD()
    {
        lastConsumeTotalEnergyTime = Time.time;
    }
}
