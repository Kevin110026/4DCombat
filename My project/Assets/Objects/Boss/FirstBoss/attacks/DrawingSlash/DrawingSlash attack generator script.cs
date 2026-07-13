using System;
using System.Collections;
using UnityEngine;

public class DrawingSlashattackgeneratorscript : AttackGenerator
{
    private Vector3 bossSwordPosition = new Vector3(-0.58f, -0.15f, 0.05f);
    private Quaternion bossSwordRotation = Quaternion.Euler(65.166f, -46.569f, 208.372f);
    private Vector3 bossSwordScale = new Vector3(1f, 1f, 1f);
    private GameObject sword;
    public UnityEngine.Object HintLightPrefab;
    private GameObject HintLight;
    private Light HintLightLight;
    private GameObject HintSound = null;
    public UnityEngine.Object SlashPerfab;
    public UnityEngine.Object TrailPerfab;
    public Material TrailParriedableMaterial;
    public Material TrailUnparriedableMaterial;
    private float hintTime;
    private float reactionTime;

    [HideInInspector] public bool invisable = false;
    [HideInInspector] public bool randomAngle = false;
    [HideInInspector] public bool unparriedable = false;
    [HideInInspector] public bool lowcd = false;
    [HideInInspector] public AudioClip parriedSound;

    void Awake()
    {
        parriedSound = Resources.Load<AudioClip>("parry-3-limbus");
    }
    protected override void Start()
    {
        base.Start();
        Vector3 deltaPos = target.transform.position - spawner.transform.position;

        sword = spawner.GetComponent<FirstBoss_script>().sword;

        sword.transform.localPosition = bossSwordPosition;
        sword.transform.localRotation = bossSwordRotation;
        sword.transform.localScale = bossSwordScale;
        HintLight = (GameObject)Instantiate(HintLightPrefab, new Vector3(0, 0, 0), new(0, 0, 0, 1));
        HintLight.transform.SetParent(sword.transform.Find("blade").transform);
        HintLight.transform.localPosition = Vector3.zero;
        HintLightLight = HintLight.GetComponent<Light>();
    }
    protected override void Update()
    {
        base.Update();
        HintLightLight.intensity *= Mathf.Pow(0.8f, Time.deltaTime * 60);
    }

    protected override IEnumerator StartAttack()
    {
        hintTime = lowcd ? Mathf.Lerp(0.5f, 0.3f, difficulty) : Mathf.Lerp(0.8f, 0.3f, difficulty) * UnityEngine.Random.Range(1, 1 + difficulty);
        reactionTime = lowcd ? Mathf.Lerp(0.28f, 0.18f, difficulty) : Mathf.Lerp(0.35f, 0.2f, difficulty);
        // if (UnityEngine.Random.Range(0f, 1f) <= 0.5)
        if (!invisable)
        {
            yield return StartCoroutine(VisableSlash());
        }
        else
        {
            yield return StartCoroutine(InvisableSlash());
        }
        Destroy(gameObject);
    }

    IEnumerator VisableSlash()
    {
        yield return new WaitForSeconds(hintTime);
        HintLightLight.intensity = 1000;
        // Animator animator = GetComponent<Animator>();
        yield return new WaitForSeconds(reactionTime);
        attackTargetLocation = target.transform.position;
        yield return StartCoroutine(Slash());
    }
    IEnumerator InvisableSlash()
    {
        // if (UnityEngine.Random.Range(0f, 1f) <= 0.5)
        if (!randomAngle)
        {
            yield return StartCoroutine(InvisableSlashStraight());
        }
        else
        {
            hintTime *= 1.3f;
            yield return StartCoroutine(InvisableSlashRandom());
        }
        yield return StartCoroutine(Slash());
    }

    IEnumerator InvisableSlashStraight()
    {
        HintSound = AttackHinterscript.NewPlayer(Resources.Load<AudioClip>("A Piano"), hintTime, reactionTime);
        float t = Time.time + hintTime + reactionTime;
        while (Time.time <= t)
        {
            HintSound.transform.position = spawner.transform.position;
            yield return null;
        }
        attackTargetLocation = target.transform.position;
    }

    private Vector3 attackTargetLocation;
    IEnumerator InvisableSlashRandom()
    {
        if (unparriedable) reactionTime *= 1.5f;

        float positionAngle = Quaternion.LookRotation(target.transform.position - spawner.transform.position).eulerAngles.y;
        positionAngle += UnityEngine.Random.Range(-90f, 90f);
        Vector3 diffLocation = Quaternion.Euler(0f, positionAngle, 0f) * Vector3.forward;
        diffLocation *= 15f;
        Vector3 attackLocation = target.transform.position - diffLocation;
        AudioClip hintSound = unparriedable ? Resources.Load<AudioClip>("D Piano") : Resources.Load<AudioClip>("A Piano");
        HintSound = AttackHinterscript.NewPlayer(hintSound, hintTime, reactionTime);
        HintSound.transform.position = attackLocation;
        float t = Time.time + hintTime;


        while (Time.time <= t)
            yield return null;
        t = Time.time + reactionTime;
        attackTargetLocation = target.transform.position;
        while (Time.time <= t)
            yield return null;
        if (!unparriedable)
            attackTargetLocation = target.transform.position;

        CharacterController characterController = spawner.GetComponent<CharacterController>();
        characterController.enabled = false;
        spawner.transform.position = attackLocation;
        characterController.enabled = true;
    }
    private GameObject tro;

    IEnumerator Slash()
    {
        GameObject slash = (GameObject)Instantiate(SlashPerfab, spawner.transform.position, Quaternion.LookRotation(attackTargetLocation - spawner.transform.position));
        DrawingSlashattackslashscript ats = slash.GetComponent<DrawingSlashattackslashscript>();
        ats.parriedSound = parriedSound;
        ats.canBeParried = !unparriedable;
        tro = (GameObject)Instantiate(TrailPerfab, spawner.transform);
        TrailRenderer trotr = tro.GetComponent<TrailRenderer>();
        trotr.material = unparriedable ? TrailUnparriedableMaterial : TrailParriedableMaterial;
        if (unparriedable)
        {
            trotr.enabled = false;
            // trotr.widthMultiplier = 2.4f;
        }
        else
        {
            trotr.widthMultiplier = 1;
        }
        LineRenderer trolr = tro.GetComponent<LineRenderer>();
        trolr.material = unparriedable ? TrailUnparriedableMaterial : TrailParriedableMaterial;
        if (unparriedable)
        {
            trolr.SetPositions(new Vector3[] { spawner.transform.position, spawner.transform.position });
            // trolr.widthMultiplier = 0f;
            trolr.widthMultiplier = 3.0f;
        }
        else
        {
            trolr.enabled = false;
        }


        yield return null;

        Vector3 deltapos = attackTargetLocation - spawner.transform.position;

        CharacterController characterController = spawner.GetComponent<CharacterController>();

        characterController.enabled = false;
        spawner.transform.position += deltapos.normalized * (deltapos.magnitude + 10);
        characterController.enabled = true;

        trolr.SetPosition(1, spawner.transform.position);

        yield return new WaitForSeconds(Mathf.Lerp(0.75f, 0.45f, difficulty) * (lowcd ? 0.0f : 1f));

        // GameObject.Destroy(tro);
    }

    void OnDestroy()
    {
        GameObject.Destroy(HintLight);
        // GameObject.Destroy(tro);
    }
}
