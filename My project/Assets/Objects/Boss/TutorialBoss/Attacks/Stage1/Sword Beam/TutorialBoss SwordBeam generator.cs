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

        yield return StartCoroutine(OneBeam(Windup1, Slash1, -7f));


        if (!justOneBeam && Random.Range(0f, 1f) < 0.9f)
        {
            yield return new WaitForSeconds(coolDownTime);
            yield return StartCoroutine(OneBeam(Windup2, Slash2, 7f));
        }

        Destroy(gameObject);
    }

    protected virtual IEnumerator OneBeam(AnimationClip WindupAnim, AnimationClip SlashAnim, float deltaDeg = 0)
    {
        GameObject hinter = AttackHinterscript.NewPlayer(ImplicitHintSound, hintTime, windowTime);
        hinter.GetComponent<AudioSource>().pitch = 0.8f;
        hinter.transform.parent = spawner.transform;
        animator.speed = WindupAnim.length / (hintTime + windowTime);
        StartCoroutine(PlayAnimationAndLoop(WindupAnim));
        yield return new WaitForSeconds(hintTime + windowTime);

        GameObject attObj = InstantiateAttack(BeamPrefab);
        attObj.GetComponent<TutorialBossSwordBeamattack>().deltaDeg = deltaDeg;
        attObj.GetComponent<TutorialBossSwordBeamattack>().energy = 4;
        tutorialBossScript.ConsumeBurstEnergy(8f);

        animator.speed = SlashAnim.length / slashTime;
        StartCoroutine(PlayAnimationAndLoop(SlashAnim));
        yield return new WaitForSeconds(slashTime);
    }

    protected virtual void OnDestroy()
    {
        animator.speed = 1;
    }
}
