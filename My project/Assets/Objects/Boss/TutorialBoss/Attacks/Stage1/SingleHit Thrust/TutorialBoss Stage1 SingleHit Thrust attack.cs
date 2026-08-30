using UnityEngine;

public class TutorialBossStage1SingleHitThrustattack : TutorialBossAttackAttack
{
    protected override void Update()
    {
        base.Update();
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
        {
            GameObject tip = tutorialBossScript.blade.transform.Find("tip").gameObject;
            float dist = (spawner.transform.position - tip.transform.position).magnitude;
            knockbackDir = spawner.transform.forward;
            RaycastHit[] hits = Physics.RaycastAll(spawner.transform.position, spawner.transform.forward, dist);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<PlayerParryBoxscript>(out var pb))
                {
                    pb.VerifyParryForAttack(this);
                }

                if (hit.collider.gameObject.TryGetComponent<PlayerScript>(out var ps) && canDamage)
                {
                    DamageCharacter(ps.gameObject);
                }
            }
            Destroy(gameObject);
        }
    }

    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 5;
        base.DamageCharacter(character);
    }
}
