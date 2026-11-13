using UnityEngine;
using UnityEngine.Audio;

public class TutorialBossGroundBurstattack : TutorialBossAttackAttack
{
    [HideInInspector] public TutorialBossGroundBurstgenerator generator;
    [SerializeField] float duration = 1f;
    float endTime;
    protected override void Start()
    {
        base.Start();
        endTime = Time.time + duration;
        generator = attackGenerator.GetComponent<TutorialBossGroundBurstgenerator>();
        knockbackDir = Vector3.up;
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= endTime)
        {
            Destroy(gameObject);
            return;
        }

        if (generator.attackedPlayerThisTick)
        {
            canBeParried = false;
            canDamage = false;
            haveDamaged = true;
            energy = 0;
            return;
        }
        else
        {
            canBeParried = true;
            canDamage = true;
            haveDamaged = false;
            energy = Time.deltaTime * 30;

            if (Time.time >= endTime)
                Destroy(this.gameObject);

            Vector3 halfExtend = new Vector3(transform.lossyScale.x / 2, 0.01f, transform.lossyScale.z / 2);
            float dist = transform.lossyScale.y / 2;
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, halfExtend, Vector3.up, Quaternion.identity, dist);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent<PlayerParryBoxscript>(out var pb))
                {
                    pb.VerifyParryForAttack(this);
                    generator.attackedPlayerThisTick = true;
                }

                if (hit.collider.gameObject.TryGetComponent<PlayerScript>(out var ps) && canDamage)
                {
                    DamageCharacter(ps.gameObject);
                    generator.attackedPlayerThisTick = true;
                }
            }
        }
    }
    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 7f;
        base.DamageCharacter(character);
    }
}
