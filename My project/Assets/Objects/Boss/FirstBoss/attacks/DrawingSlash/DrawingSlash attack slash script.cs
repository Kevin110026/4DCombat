using Unity.VisualScripting;
using UnityEngine;

public class DrawingSlashattackslashscript : AttackScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        RaycastHit[] hits;
        if (canBeParried) hits = Physics.RaycastAll(transform.position, transform.forward, 50f);
        else hits = Physics.SphereCastAll(transform.position, 3.0f, transform.forward, 50f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
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
