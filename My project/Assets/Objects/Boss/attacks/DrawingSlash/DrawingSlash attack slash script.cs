using Unity.VisualScripting;
using UnityEngine;

public class DrawingSlashattackslashscript : Attackscript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RaycastHit[] hits;
        if (canBeParried) hits = Physics.RaycastAll(transform.position, transform.forward, 50f);
        else hits = Physics.SphereCastAll(transform.position, 3.0f, transform.forward, 50f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent<PlayerParryBoxscript>(out var pb))
            {
                pb.VerifyParryForAttack(this);
            }

            if (hit.collider.gameObject.TryGetComponent<Playerscript>(out var ps) && canDamage)
            {
                DamagePlayer(ps.gameObject);
            }
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnParried(GameObject parrier)
    {
        base.OnParried(parrier);
        parrier.GetComponent<CharacterMovementScript>().AddVelocity(3f * transform.forward);
        Destroy(gameObject);
    }
}
