using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class FlyingSwordsattackswordscript : AttackScript
{
    [HideInInspector] public float speed;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float maxAcceleration;
    [HideInInspector] public float decay = 0.7f;
    [HideInInspector] public bool visible = true;
    public AudioClip sound;
    private AudioSource audioSource;
    private Renderer theRenderer;
    [HideInInspector] public bool speedUp = false;
    private bool attacking = false;
    private int attackCount = 0;
    private float rngRotateDiff = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        damage = 25;
        canDamage = false;
        haveDamaged = false;
        canBeParried = false;
        audioSource = GetComponent<AudioSource>();
        // meshRenderer = GetComponent<MeshRenderer>();
        theRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        Vector3 posDiff = target.transform.position - transform.position;
        float derDot = Vector3.Dot(posDiff.normalized, transform.forward);


        if (posDiff.magnitude >= 0.75f * GetMaxSpeed(attackGenerator.difficulty))
        {
            if (derDot >= Mathf.Sqrt(3) / 2)
            {
                if (!attacking)
                    StartAttack();
            }
            else
            {
                if (derDot < 0f)
                {
                    EndAttack();
                    if (attackCount >= 2)
                        Destroy(gameObject);
                }
            }
        }
        else
        {
            speedUp = true;
        }

        // Debug.Log(attacking);


        if (speedUp)
        {
            speed += Time.deltaTime * GetMaxAcceleration(attackGenerator.difficulty);
        }
        else
        {
            speed -= Time.deltaTime * GetMaxAcceleration(attackGenerator.difficulty);
        }

        speed = Mathf.Clamp(speed, 3, GetMaxSpeed(attackGenerator.difficulty));

        if (!attacking && rngRotateDiff == 1)
        {
            rngRotateDiff = Random.Range(0.5f, 1.5f);
        }
        else
        {
            rngRotateDiff = 1;
        }

        Vector3 targetDir = target.transform.position - transform.position;
        float maxRadiansDelta = 4 * GetMaxAcceleration(attackGenerator.difficulty) * rngRotateDiff / (speedUp ? 2 : 1) / speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, maxRadiansDelta, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        transform.position += transform.forward * speed * Time.deltaTime;

        if ((target.transform.position - transform.position).magnitude > 3 && !visible)
        {
            GetComponent<Renderer>().enabled = false;
        }
        else
        {
            GetComponent<Renderer>().enabled = true;
        }
        if (transform.position.y <= 0.2f)
        {
            Vector3 tmp = transform.position;
            tmp.y = Mathf.Max(0.2f, tmp.y);
            transform.position = tmp;

            Vector3 flattenedForward = transform.forward;
            flattenedForward.y = 0;

            if (flattenedForward != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(flattenedForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
        }



        if (!visible)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.enabled = true;
                audioSource.Play();
            }
            if (audioSource.time >= 0.7f)
            {
                audioSource.time = 0.1f;
            }
            if (posDiff.magnitude / GetMaxSpeed(attackGenerator.difficulty) <= 0.5f && canDamage)
            {
                audioSource.volume = 0;
            }
            else
            {
                audioSource.volume = 1;
            }
            if (posDiff.magnitude <= 3)
            {
                ShowVisually(true);
            }
            else
            {
                ShowVisually(false);
            }
        }
    }
    public void StartAttack()
    {
        attacking = true;
        speedUp = true;
        canDamage = true;
        canBeParried = true;
        haveDamaged = false;
        attackCount++;
    }
    public void EndAttack()
    {
        attacking = false;
        speedUp = false;
        canDamage = false;
        canBeParried = false;
        haveDamaged = false;
    }
    public float GetMaxSpeed(float difficulity)
    {
        return (speedUp ? 2 : 1) * Mathf.Lerp(difficulity, 12, 20);
    }
    public float GetMaxAcceleration(float difficulity)
    {
        return (speedUp ? 1 : 2) * Mathf.Lerp(difficulity, 12 * 0.6f, 20 * 0.6f);
    }

    public void ShowVisually(bool show)
    {
        // 取得物件本身與所有子物件的 Renderer
        Renderer[] renders = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renders)
        {
            r.enabled = show;
        }

        GetComponent<Renderer>().enabled = show;
    }


    virtual protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerScript>(out PlayerScript player))
        {
            if (canDamage)
            {
                DamageCharacter(other.gameObject);
            }
        }
        else if (other.TryGetComponent<PlayerParryBoxscript>(out PlayerParryBoxscript parryBox))
        {
            parryBox.VerifyParryForAttack(this);
        }
    }

    void OnDestroy()
    {
        if (attackGenerator != null && attackGenerator.TryGetComponent(out FlyingSwordsattackgeneratorscript c))
        {
            if (c != null)
                c.remainingSwords--;
        }
        audioSource.Stop();
    }

    public override void OnParried(GameObject parrier)
    {
        base.OnParried(parrier);
        if (attackCount >= 2)
            Destroy(gameObject);
    }
    protected override void DamageCharacter(GameObject player)
    {
        base.DamageCharacter(player);
        if (attackCount >= 2)
            Destroy(gameObject);
    }
}
