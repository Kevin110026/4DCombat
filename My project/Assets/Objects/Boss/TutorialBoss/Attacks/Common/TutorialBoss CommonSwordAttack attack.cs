using UnityEngine;

public class TutorialBossCommonSwordAttackattack : AttackScript
{
    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerScript>(out PlayerScript player))
        {
            if (canDamage)
            {
                DamageCharacter(other.gameObject);
            }
        }
        else if (other.TryGetComponent<PlayerParryBoxscript>(out PlayerParryBoxscript parryBox))
        {
            parryBox.VerifyParryForAttack(this);
        }
    }
    protected override void DamageCharacter(GameObject player)
    {
        damage = energy * 5;
        base.DamageCharacter(player);
    }
}
