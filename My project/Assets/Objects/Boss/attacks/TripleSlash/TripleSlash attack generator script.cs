using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(AudioSource))]
public class TripleSlashattackgeneratorscript : attackgenerator
{

    public UnityEngine.Object SlashPrefab;
    public AudioClip ExplicitHintSound;
    public AudioClip ImplicitHintSound;
    public AudioClip SlashWhoosh;

    public UnityEngine.Object HintLightPrefab;
    private GameObject HintLight;
    private Light HintLightLight;

    private AudioSource audioSource;
    private GameObject ImplicitHintPlayer;
    private float thirdAttackStartTime;
    private float thirdAttackOriginalAngle;
    private float thirdAttackAngle;
    private float thirdAttackAngleSpin;
    private Vector3 thirdAttackFace;

    private Vector3 bossSwordPosition = new Vector3(-0.58f, -0.15f, 0.05f);
    private Quaternion bossSwordRotation = Quaternion.Euler(65.166f, -46.569f, 208.372f);
    private Vector3 bossSwordScale = new Vector3(1f, 1f, 1f);
    private GameObject sword;

    private float implicitHintTime;
    private float implicitHintReactionTime;
    private float hintReactionTime;
    private float slashTimeSeperate;
    private int slashCount;



    protected override void Start()
    {
        base.Start();
        Vector3 deltaPos = target.transform.position - spawner.transform.position;
        // deltaPos = deltaPos.normalized*Mathf.Clamp
        // spawner.transform.position

        // startHint = false;
        sword = spawner.GetComponent<Boss_script>().sword;

        sword.transform.localPosition = bossSwordPosition;
        sword.transform.localRotation = bossSwordRotation;
        sword.transform.localScale = bossSwordScale;
        HintLight = (GameObject)Instantiate(HintLightPrefab, new Vector3(0, 0, 0), new(0, 0, 0, 1));
        HintLight.transform.SetParent(sword.transform.Find("blade").transform);
        HintLight.transform.localPosition = Vector3.zero;
        HintLightLight = HintLight.GetComponent<Light>();
    }

    override protected void Update()
    {
        base.Update();
        HintLightLight.intensity *= Mathf.Pow(0.8f, Time.deltaTime * 60);
    }

    protected override IEnumerator StartAttack()
    {
        //  move to desired position
        CharacterMovementScript movement = spawner.GetComponent<CharacterMovementScript>();
        Vector3 relativePosition = spawner.transform.position - target.transform.position;
        relativePosition.y = 0;
        relativePosition = relativePosition.normalized * 10f;
        relativePosition = Quaternion.Euler(0, UnityEngine.Random.Range(-50f, 50f), 0) * relativePosition;
        Vector2 randomShift = UnityEngine.Random.insideUnitCircle * 3f;
        relativePosition.x += randomShift.x;
        relativePosition.z += randomShift.y;
        relativePosition.y = spawner.transform.position.y - target.transform.position.y;
        movement.MoveToXZ(relativePosition + target.transform.position, 30);

        while (!movement.doneMoveTo)
        {
            // Debug.Log
            yield return null;
        }


        //  starts the attack
        implicitHintTime = Mathf.Lerp(1.2f, 0.6f, difficulity);
        implicitHintReactionTime = Mathf.Lerp(0.35f, 0.23f, difficulity);
        hintReactionTime = Mathf.Lerp(0.33f, 0.22f, difficulity);
        slashTimeSeperate = Mathf.Lerp(0.55f, 0.35f, difficulity);
        slashCount = (int)(Mathf.Lerp(3, 5, difficulity) + (UnityEngine.Random.Range(0, difficulity) >= 0.5f ? 1 : 0));


        transform.position = spawner.transform.position;
        audioSource = GetComponent<AudioSource>();

        thirdAttackOriginalAngle = Quaternion.LookRotation(target.transform.position - spawner.transform.position).eulerAngles.y;
        thirdAttackAngle = thirdAttackOriginalAngle + UnityEngine.Random.Range(-90, 90);
        thirdAttackAngleSpin = 0;
        thirdAttackStartTime = Time.time;

        List<float> slashTimeStamps = new List<float>(slashCount);
        float t = 0.0f + hintReactionTime;
        for (int i = 0; i < slashCount; i++)
        {
            slashTimeStamps.Add(t + slashTimeSeperate * UnityEngine.Random.Range(0.8f, 1.1f));
            t = slashTimeStamps[i];
        }

        for (int i = 0; i < slashCount - 1; i++)
        {
            StartCoroutine(StartNormalSlash(slashTimeStamps[i] - hintReactionTime));
        }
        
        yield return
        StartCoroutine(StartImplicitSlash(slashTimeStamps[slashCount - 1] - implicitHintTime - implicitHintReactionTime));

        GameObject.Destroy(gameObject);
    }


    IEnumerator StartNormalSlash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        HintLightLight.intensity = 1000;
        yield return new WaitForSeconds(hintReactionTime);
        audioSource.PlayOneShot(SlashWhoosh);
        SpawnSlash();
    }
    IEnumerator StartImplicitSlash(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        float startTime = Time.time;
        ImplicitHintPlayer = AttackHinterscript.NewPlayer(ImplicitHintSound, implicitHintTime, implicitHintReactionTime, 0f);
        while (Time.time <= startTime + implicitHintTime + implicitHintReactionTime)
        {
            thirdAttackAngleSpin = Mathf.Lerp(thirdAttackOriginalAngle, thirdAttackAngle, (Time.time - thirdAttackStartTime) / implicitHintTime);
            thirdAttackFace = Quaternion.Euler(0, thirdAttackAngleSpin, 0) * Vector3.forward;
            ImplicitHintPlayer.transform.position = target.transform.position - 10.0f * thirdAttackFace;
            yield return null;
        }
        audioSource.PlayOneShot(SlashWhoosh);
        SpawnThirdSlash();
    }


    void SpawnSlash()
    {
        Vector3 face = (target.transform.position - spawner.transform.position).normalized;
        UnityEngine.Object slash1 = Instantiate(SlashPrefab, spawner.transform.position, Quaternion.LookRotation(target.transform.position - spawner.transform.position));
        slash1.GetComponent<Rigidbody>().linearVelocity = face * 150.0f;
        slash1.GetComponent<Attackscript>().parriedSound = Resources.Load<AudioClip>("parry-1-limbus");
    }
    void SpawnThirdSlash()
    {
        thirdAttackFace = Quaternion.Euler(0, thirdAttackAngleSpin, 0) * Vector3.forward;
        Vector3 position = target.transform.position - 10.0f * thirdAttackFace;
        UnityEngine.Object slash1 = Instantiate(SlashPrefab, position, Quaternion.Euler(0, thirdAttackAngle, 0));
        slash1.GetComponent<Rigidbody>().linearVelocity = thirdAttackFace * 150.0f;
        slash1.GetComponent<Attackscript>().parriedSound = Resources.Load<AudioClip>("parry-3-limbus");
    }

    void OnDestroy()
    {
        GameObject.Destroy(HintLight);
    }
}
