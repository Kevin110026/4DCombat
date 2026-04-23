using System;
using System.Collections;
using UnityEngine;

public class DrawingSlashattackgeneratorscript : attackgenerator
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

    protected override void Start()
    {
        base.Start();
        Vector3 deltaPos = target.transform.position - spawner.transform.position;

        sword = spawner.GetComponent<Boss_script>().sword;

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
        if (HintSound != null)
        {
            HintSound.transform.position = spawner.transform.position;
        }
    }

    protected override IEnumerator StartAttack()
    {
        float hintTime = Mathf.Lerp(0.8f, 0.3f, difficulity);
        float reactionTime = Mathf.Lerp(0.35f, 0.22f, difficulity);
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            yield return new WaitForSeconds(hintTime);
            HintLightLight.intensity = 1000;
            yield return new WaitForSeconds(reactionTime);
        }
        else
        {
            HintSound = AttackHinterscript.NewPlayer(Resources.Load<AudioClip>("A Piano"), hintTime, reactionTime);
            yield return new WaitForSeconds(hintTime + reactionTime);
        }
        yield return StartCoroutine(Slash());
        Destroy(gameObject);
    }

    IEnumerator Slash()
    {
        GameObject strike = (GameObject)Instantiate(SlashPerfab, spawner.transform.position, Quaternion.LookRotation(target.transform.position - spawner.transform.position));
        Attackscript ats = strike.GetComponent<Attackscript>();
        ats.parriedSound = Resources.Load<AudioClip>("parry-1-limbus");

        GameObject tro = (GameObject)Instantiate(TrailPerfab, spawner.transform);
        GameObject slash = (GameObject)Instantiate(SlashPerfab, spawner.transform.position, spawner.transform.rotation);
        yield return null;

        // Physics.IgnoreCollision(spawner.GetComponent<Collider>(), target.GetComponent<Collider>(), true);
        // Physics.SyncTransforms();

        Vector3 deltapos = target.transform.position - spawner.transform.position;

        CharacterController characterController = spawner.GetComponent<CharacterController>();

        characterController.enabled = false;
        spawner.transform.position += deltapos.normalized * (deltapos.magnitude + 10);
        characterController.enabled = true;

        // Physics.SyncTransforms();
        // Physics.IgnoreCollision(spawner.GetComponent<Collider>(), target.GetComponent<Collider>(), false);

        yield return new WaitForSeconds(Mathf.Lerp(0.75f, 0.45f, difficulity));

        GameObject.Destroy(tro);
    }

    void OnDestroy()
    {
        GameObject.Destroy(HintLight);
    }
}
