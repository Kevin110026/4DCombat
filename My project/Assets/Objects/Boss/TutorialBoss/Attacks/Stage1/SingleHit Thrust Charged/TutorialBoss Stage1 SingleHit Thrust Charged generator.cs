using System.Collections;
using UnityEngine;

public class TutorialBossStage1SingleHitThrustChargedgenerator : TutorialBossCommonSwordAttackChargedgenerator
{
    [SerializeField] Object attPerfab;
    // protected override void AttackLoopAction()
    // {
    //     CharacterController cc = spawner.GetComponent<CharacterController>();
    //     float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
    //     float maxd = 25f * Time.deltaTime;
    //     float moved = Mathf.Clamp(dist, -maxd, maxd);
    //     cc.Move(moved * spawner.transform.forward);
    // }
    protected override IEnumerator Attack()
    {
        GameObject attObj = InstantiateAttack(attPerfab);
        attObj.GetComponent<TutorialBossStage1SingleHitThrustChargedattack>().energy = 4.5f;
        attObj.GetComponent<TutorialBossStage1SingleHitThrustChargedattack>().knockbackMag = knockbackMag;
        tutorialBossScript.ConsumeBurstEnergy(10f);

        animator.speed = WindupAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(WindupAnimation));

        isSlashing = true;

        TutorialBossCommonSwordAttackChargedattack fakeAtt = InstantiateAttack<TutorialBossCommonSwordAttackChargedattack>(blade);
        fakeAtt.energy = 0;
        fakeAtt.canDamage = false;
        fakeAtt.canBeParried = false;


        AudioClip slash_sound = Resources.Load<AudioClip>("dragon-studio-whoosh-motion-405445");
        AudioSource blade_audioSource = blade.GetComponent<AudioSource>();
        blade_audioSource.clip = slash_sound;
        blade_audioSource.pitch = Random.Range(0.95f, 1.05f);
        blade_audioSource.time = 0.3f;
        blade_audioSource.volume = 0.6f;
        blade_audioSource.Play();


        animator.speed = AttackAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(AttackAnimation, SlashLoopAction));

        isSlashing = false;

        yield return new WaitForSeconds(0.04f);
        this.ScheduleDestroy(attObj);
        this.ScheduleDestroy(fakeAtt);

        animator.speed = 1;
    }
}
