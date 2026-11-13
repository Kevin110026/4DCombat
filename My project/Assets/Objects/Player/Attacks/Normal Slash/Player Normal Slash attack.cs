using UnityEngine;

public class PlayerNormalSlashattack : PlayerAttackAttack
{
    [SerializeField] float PushSpeed = 10f;
    [SerializeField] float PushTimeLen = 0.05f;
    Vector3 pushDir;
    float pushTimeRemain;
    protected override void Update()
    {
        base.Update();
        if (pushTimeRemain > 0f)
        {
            float dt = Mathf.Min(pushTimeRemain, Time.deltaTime);
            spawner.GetComponent<CharacterController>().Move(PushSpeed * dt * pushDir);
            pushTimeRemain -= dt;
        }
    }
    virtual protected void OnTriggerStay(Collider other)
    {
        if (!canDamage)
            return;
        GameObject otherObj = other.gameObject;
        while (otherObj != null)
        {
            if (otherObj.TryGetComponent<CharacterScript>(out CharacterScript characterScript))
            {
                if (spawner != otherObj && canDamage)
                {
                    DamageCharacter(otherObj);
                    break;
                }
            }
            if (otherObj.transform.parent == null)
                break;
            else
                otherObj = otherObj.transform.parent.gameObject;
        }
        // else if (other.TryGetComponent<PlayerParryBoxscript>(out PlayerParryBoxscript parryBox))
        // {
        //     parryBox.VerifyParryForAttack(this);
        // }
    }
    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 5;
        base.DamageCharacter(character);
        pushDir = (spawner.transform.position - character.transform.position).normalized;
        pushTimeRemain = PushTimeLen;

       
    }
}
