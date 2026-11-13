using UnityEngine;

public class PlayerDashSlashattack : PlayerAttackAttack
{
    Vector3 dir;
    float speed;
    protected override void Start()
    {
        base.Start();

        speed = attackGenerator.GetComponent<PlayerDashSlashgenerator>().speed;
        dir = attackGenerator.GetComponent<PlayerDashSlashgenerator>().dir;

    }

    protected override void Update()
    {
        base.Update();

        Vector3 ori = spawner.transform.position;
        Vector3 dir = spawner.transform.forward;
        RaycastHit[] hits = Physics.BoxCastAll(ori, new Vector3(5f, 2.5f, 0.01f), dir, Quaternion.identity, speed * Time.deltaTime + 2.5f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.TryGetComponent<CharacterScript>(out CharacterScript characterScript))
            {
                if (spawner != hitObj && canDamage)
                {
                    DamageCharacter(hitObj);
                    attackGenerator.GetComponent<PlayerDashSlashgenerator>().stopPoint =
                        ori + dir * hit.distance;
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    protected override void DamageCharacter(GameObject character)
    {
        damage = energy * 8;
        base.DamageCharacter(character);
        
    }
}
