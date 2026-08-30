using UnityEngine;

public class TrainDummyNormalHitattack : TrainDummyAttack
{
    protected override void Start()
    {
        base.Start();

        knockbackDir = spawner.transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(spawner.transform.position, spawner.transform.forward, 2f);
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

    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 5;
        base.DamageCharacter(character);
    }
}
