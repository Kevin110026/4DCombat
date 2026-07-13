using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialBossWhirlwindattack : TutorialBossCommonSwordAttackattack
{
    [SerializeField] private float AttackCD = 0.33f;
    public float BaseEnergy = 2.5f;
    public float EnergyCost = 4.5f;
    private float CDEndTime;
    protected override void Start()
    {
        base.Start();
        CDEndTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();
    }
    public bool CDEnd() { return Time.time >= CDEndTime; }

    public void RefreshAttack()
    {
        spawner.GetComponent<CharacterScript>().ConsumeBurstEnergy(EnergyCost);
        energy = BaseEnergy;
        haveDamaged = false;
        canDamage = true;
        canBeParried = true;
    }
    public override void OnParried(GameObject parrier)
    {
        base.OnParried(parrier);
        CDEndTime = Time.time + AttackCD;
    }
    protected override void DamageCharacter(GameObject player)
    {
        base.DamageCharacter(player);
        CDEndTime = Time.time + AttackCD;
    }
}
