using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBossStage1ChainLRCTgenerator : TutorialBossAttackGenerator
{
    public Object LeftHit;
    public Object RightHit;
    public Object ThrustHitCharged;

    private static readonly float[][] hitTimes = new float[][] {
        new float[] { 0.5f, 1.0f },
        new float[] { 1.0f, 0.5f }
        };

    protected override IEnumerator StartAttack()
    {
        int type = Random.Range(0, 2);
        float hitTime = 0.5f;
        StartCoroutine(HitAfterSecond(hitTime, LeftHit, 2.0f, 2.0f));
        hitTime += hitTimes[type][0];
        StartCoroutine(HitAfterSecond(hitTime, RightHit, 1.2f, 2.0f));
        hitTime += hitTimes[type][1];
        yield return StartCoroutine(HitAfterSecond(hitTime, ThrustHitCharged, 4f, 10f, 0.5f, 0.2f));

        Destroy(gameObject);
    }

    IEnumerator HitAfterSecond(float seconds, Object attackGenObj, float WindupSpeed, float AttackSpeed)
    {
        float offset = seconds;
        TutorialBossAnimatedAttack attack = attackGenObj.GetComponent<TutorialBossAnimatedAttack>();
        offset -= attack.WindupAnimation.length / WindupSpeed;
        offset -= attack.AttackAnimation.length / AttackSpeed / 2;
        float spawnTime = Time.time + offset;

        while (Time.time < spawnTime)
            yield return null;

        GameObject curGenObj;
        TutorialBossAnimatedAttack curGen;
        curGenObj = InstantiateGenerator(attackGenObj);
        curGen = curGenObj.GetComponent<TutorialBossAnimatedAttack>();
        curGen.WindupAnimationPlaySpeed = WindupSpeed;
        curGen.AttackAnimationPlaySpeed = AttackSpeed;
        while (curGenObj != null)
            yield return null;

    }
    IEnumerator HitAfterSecond(float seconds, Object attackGenObj, float WindupSpeed, float AttackSpeed, float HintTime, float SpaceTime)
    {
        float offset = seconds;
        TutorialBossAnimatedAttack attack = attackGenObj.GetComponent<TutorialBossAnimatedAttack>();
        offset -= HintTime;
        offset -= SpaceTime;
        offset -= attack.WindupAnimation.length / WindupSpeed;
        offset -= attack.AttackAnimation.length / AttackSpeed / 2;
        float spawnTime = Time.time + offset;

        while (Time.time < spawnTime)
            yield return null;
            
        GameObject curGenObj;
        TutorialBossCommonSwordAttackChargedgenerator curGen;
        curGenObj = InstantiateGenerator(attackGenObj);
        curGen = curGenObj.GetComponent<TutorialBossCommonSwordAttackChargedgenerator>();
        curGen.WindupAnimationPlaySpeed = WindupSpeed;
        curGen.AttackAnimationPlaySpeed = AttackSpeed;
        curGen.soundHintTime = HintTime;
        curGen.soundSpaceTime = SpaceTime;
        while (curGenObj != null)
            yield return null;

    }
}
