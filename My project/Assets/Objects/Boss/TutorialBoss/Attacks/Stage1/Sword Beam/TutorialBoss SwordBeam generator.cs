using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBossSwordBeamgenerator : TutorialBossAttackGenerator
{
    [SerializeField] Object BackOffPrefab;
    [SerializeField] Object BeamPrefab;
    [SerializeField] AnimationClip Windup1;
    [SerializeField] AnimationClip Slash1;
    [SerializeField] AnimationClip Windup2;
    [SerializeField] AnimationClip Slash2;
    [SerializeField] AudioClip ImplicitHintSound;
    // [SerializeField] float 

    [SerializeField] float hintTime = 0.3f;
    [SerializeField] float windowTime = 0.2f;
    [SerializeField] float slashTime = 0.1f;
    [SerializeField] float coolDownTime = 0.1f;
    [SerializeField] HintSoundPath leftHintPath;
    [SerializeField] HintSoundPath rightHintPath;

    [HideInInspector] bool justOneBeam = false;

    protected override IEnumerator StartAttack()
    {
        float dist = (spawner.transform.position - target.transform.position).magnitude;
        if (dist < 14f)
        {
            GameObject bo = InstantiateGenerator(BackOffPrefab);
            bo.GetComponent<TutorialBossMovementBackOff>().BackOffDistance = 20f - dist;
            while (bo != null)
                yield return null;
        }

        yield return StartCoroutine(OneBeam(Windup1, Slash1, -7f, leftHintPath));


        if (!justOneBeam && Random.Range(0f, 1f) < 0.9f)
        {
            yield return new WaitForSeconds(coolDownTime);
            yield return StartCoroutine(OneBeam(Windup2, Slash2, 7f, rightHintPath));
        }

        Destroy(gameObject);
    }
    GameObject hinterCarrier;
    protected virtual IEnumerator OneBeam(AnimationClip WindupAnim, AnimationClip SlashAnim, float deltaDeg, HintSoundPath hintPath)
    {
        GameObject hinter = AttackHinterscript.NewPlayer(ImplicitHintSound, hintTime, windowTime);
        hinter.GetComponent<AudioSource>().pitch = 0.8f;
        hinterCarrier = new GameObject("HinterCarrier");
        LoadHintSoundPath(hinter, hinterCarrier, hintPath, hintTime);
        Vector3 rot = Quaternion.LookRotation(spawner.transform.position - target.transform.position).eulerAngles;
        rot.x = 0; rot.z = 0;
        hinter.transform.rotation = Quaternion.Euler(rot);

        // hinter.transform.parent = spawner.transform;
        animator.speed = WindupAnim.length / (hintTime + windowTime);
        StartCoroutine(PlayAnimationAndLoop(WindupAnim));
        yield return LoopForSeconds(hintTime + windowTime, LocateHinter);
        // yield return new WaitForSeconds(hintTime + windowTime);

        GameObject attObj = InstantiateAttack(BeamPrefab);
        attObj.GetComponent<TutorialBossSwordBeamattack>().deltaDeg = deltaDeg;
        attObj.GetComponent<TutorialBossSwordBeamattack>().energy = 4;
        tutorialBossScript.ConsumeBurstEnergy(8f);

        animator.speed = SlashAnim.length / slashTime;
        StartCoroutine(PlayAnimationAndLoop(SlashAnim, tutorialBossScript.FaceTarget));
        yield return new WaitForSeconds(slashTime);
        Destroy(hinterCarrier);
    }
    void LocateHinter()
    {
        GameObject carrier = hinterCarrier;
        carrier.transform.position = target.transform.position;
        Vector3 rot = Quaternion.LookRotation(carrier.transform.position - spawner.transform.position).eulerAngles;
        rot.x = 0; rot.z = 0;
        carrier.transform.rotation = Quaternion.Euler(rot);
    }

    protected virtual void OnDestroy()
    {
        animator.speed = 1;
        if (hinterCarrier != null)
            Destroy(hinterCarrier);
    }
}
