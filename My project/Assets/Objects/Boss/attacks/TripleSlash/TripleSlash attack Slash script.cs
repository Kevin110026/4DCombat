using UnityEngine;

public class TripleSlashattackSlashscript : Attackscript
{
    private new Rigidbody rigidbody;
    private float spawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnTime = Time.time;
        // rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - spawnTime >= 1)
            Object.Destroy(gameObject);
    }


    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Playerscript>(out Playerscript player))
        {
            if (canDamage)
            {
                DamagePlayer(other.gameObject);
            }
        }
        else if (other.TryGetComponent<PlayerParryBoxscript>(out PlayerParryBoxscript parryBox))
        {
            parryBox.VerifyParryForAttack(this);
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
