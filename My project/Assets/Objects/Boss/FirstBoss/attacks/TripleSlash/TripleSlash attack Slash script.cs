using UnityEngine;

public class TripleSlashattackSlashscript : AttackScript
{
    private new Rigidbody rigidbody;
    private float spawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        spawnTime = Time.time;
        // rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        if (Time.time - spawnTime >= 1)
            Object.Destroy(gameObject);
    }


    virtual protected void OnTriggerEnter(Collider other)
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

    public override void OnParried(GameObject parrier)
    {
        base.OnParried(parrier);
        parrier.GetComponent<CharacterMovementScript>().AddVelocity(5f * transform.forward);
        transform.Find("Trail").transform.SetParent(null);
        Object.Destroy(gameObject);
    }
}
