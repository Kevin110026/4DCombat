using UnityEngine;

public class TutorialBossCommonSwordAttackChargedattack : TutorialBossAttackAttack
{

    public Material trailMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector3 blade_scale;

    private SwordTrail swordTrail;

    protected override void Start()
    {
        base.Start();
        blade_scale = blade.transform.localScale;

        swordTrail = sword.transform.Find("trail").GetComponent<SwordTrail>();
        swordTrail.ActivateAll();
    }

    Vector3 lastPos;
    Quaternion lastRot;

    protected override void Update()
    {
        base.Update();
        animator.Update(0);
        Vector3 curPos = blade.transform.position;
        Quaternion curRot = blade.transform.rotation;
        if (lastPos != null)
        {
            Vector3 dir = curPos - lastPos;
            knockbackDir = dir;
            Quaternion midRot = Quaternion.Slerp(lastRot, curRot, 0.5f);
            RaycastHit[] hits = Physics.BoxCastAll(lastPos, blade_scale / 2, dir, midRot, dir.magnitude);
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
            foreach (RaycastHit hit in hits)
            {
                OnRaycastHit(hit.collider);
            }
        }
        lastPos = curPos;
        lastRot = curRot;
    }


    virtual protected void OnRaycastHit(Collider other)
    {
        if (other.TryGetComponent<PlayerScript>(out PlayerScript player))
        {
            if (canDamage)
            {
                OnHitCharacter(other.gameObject);
            }
        }
        else if (other.TryGetComponent<PlayerParryBoxscript>(out PlayerParryBoxscript parryBox))
        {
            OnHitParryBox(parryBox);
        }
    }
    protected override void DamageCharacter(GameObject player)
    {
        damage = energy * 5;
        base.DamageCharacter(player);
    }

    void OnDestroy()
    {
        swordTrail.DeactivateAll();
    }
}
