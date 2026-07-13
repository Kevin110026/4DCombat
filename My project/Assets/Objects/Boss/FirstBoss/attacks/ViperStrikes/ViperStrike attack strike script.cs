using Unity.VisualScripting;
using UnityEngine;

public class ViperStrikeattackstrikescript : AttackScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 10f);
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

    // Update is called once per frame
    override protected void Update()
    {

    }

    public override void OnParried(GameObject parrier)
    {
        base.OnParried(parrier);
        parrier.GetComponent<CharacterMovementScript>().AddVelocity(3f * transform.forward);
        Destroy(gameObject);
    }
}
