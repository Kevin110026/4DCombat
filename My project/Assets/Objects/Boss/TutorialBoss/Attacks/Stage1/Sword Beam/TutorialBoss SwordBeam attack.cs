using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialBossSwordBeamattack : TutorialBossAttackAttack
{
    [HideInInspector] public float deltaDeg;
    public float speed = 40f;
    public float lifeTime = 1.5f;
    float endTime;
    protected override void Start()
    {
        base.Start();
        transform.position = spawner.transform.position;
        Vector3 rot = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
        rot.x = 0;
        rot.y += deltaDeg;
        rot.z = deltaDeg * 2;
        transform.rotation = Quaternion.Euler(rot);
        endTime = Time.time + lifeTime;
    }
    protected virtual void LateUpdate()
    {
        if (Time.time > endTime)
        {
            Destroy(gameObject);
            return;
        }
        float tarRotY = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles.y;
        Vector3 tarRot = transform.rotation.eulerAngles;
        tarRot.y = tarRotY;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(tarRot), 30f * Time.deltaTime);
        // float delta = Mathf.Clamp(tarRotY - oriRotY, -, 30f * Time.deltaTime);
        // oriRot.y = oriRotY + delta;
        // transform.rotation = Quaternion.Euler(oriRot);

        float dist = speed * Time.deltaTime;
        knockbackDir = transform.forward;
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.lossyScale / 2, transform.forward, transform.rotation, dist);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent<PlayerParryBoxscript>(out var pb))
            {
                OnHitParryBox(pb);
            }

            if (hit.collider.gameObject.TryGetComponent<PlayerScript>(out var ps) && canDamage)
            {
                OnHitCharacter(ps.gameObject);
            }
        }
        transform.position += transform.forward * dist;
    }

    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 4;
        base.DamageCharacter(character);
    }
}
