using UnityEngine;

public class TutorialBossCommonSwordAttackattack : TutorialBossAttackAttack
{
    Vector3 blade_scale;

    protected override void Start()
    {
        base.Start();
        blade_scale = blade.transform.localScale;
    }

    Vector3 lastPos;
    Quaternion lastRot;

    protected override void Update()
    {
        base.Update();
        Vector3 curPos = blade.transform.position;
        Quaternion curRot = blade.transform.rotation;
        if (lastPos != null)
        {
            Vector3 dir = curPos - lastPos;
            knockbackDir = dir;
            RaycastHit[] hits = Physics.BoxCastAll(lastPos, blade_scale / 2, dir, lastRot, dir.magnitude);
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
            foreach (RaycastHit hit in hits)
            {
                OnTriggerEnter(hit.collider);
            }
        }
        lastPos = curPos;
        lastRot = curRot;
    }

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
