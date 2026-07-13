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

    int possCnt = 0;

    protected override void Update()
    {
        base.Update();
        int possCntNew = swordTrail.trailRenderer.positionCount;
        while (possCnt < possCntNew)
        {
            possCnt++;
            if (possCnt == 1)
                continue;
            Vector3 s = swordTrail.trailRenderer.GetPosition(possCnt - 2);
            Vector3 e = swordTrail.trailRenderer.GetPosition(possCnt - 1);
            Vector3 dir = e - s;
            RaycastHit[] hits = Physics.BoxCastAll(s, blade_scale / 2, dir, Quaternion.LookRotation(dir), dir.magnitude);
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
            foreach (RaycastHit hit in hits)
            {
                OnTriggerEnter(hit.collider);
            }
        }
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

    void OnDestroy()
    {
        swordTrail.DeactivateAll();
    }
}
