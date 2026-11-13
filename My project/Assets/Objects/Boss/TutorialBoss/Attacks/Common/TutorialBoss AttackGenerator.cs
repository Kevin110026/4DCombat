using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossAttackGenerator : AttackGenerator
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected TutorialBossScript tutorialBossScript;
    protected List<GameObject> childObjects = new();
    // protected AnimatorStateInfo animatorStateInfo;
    protected override void Start()
    {
        animator = spawner.GetComponent<Animator>();
        tutorialBossScript = spawner.GetComponent<TutorialBossScript>();
        sword = tutorialBossScript.sword;
        blade = tutorialBossScript.blade;
        base.Start();
    }
    protected IEnumerator HitAfterSecond(float seconds, GameObject attackGenObj, float WindupSpeed, float AttackSpeed, float parriedPitch = 1.0f, DataWrapper<GameObject> retObj = null)
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
        // childObjects.Add(curGenObj);
        curGen = curGenObj.GetComponent<TutorialBossAnimatedAttack>();
        curGen.WindupAnimationPlaySpeed = WindupSpeed;
        curGen.AttackAnimationPlaySpeed = AttackSpeed;
        curGen.parriedPitch = parriedPitch;
        if (retObj != null)
            retObj.Val = curGenObj;
        while (curGenObj != null)
            yield return null;

    }
    protected IEnumerator HitAfterSecond(float seconds, GameObject attackGenObj, float WindupSpeed, float AttackSpeed, float HintTime, float SpaceTime, float windupTime, float parriedPitch = 1.0f, DataWrapper<GameObject> retObj = null)
    {
        float offset = seconds;
        TutorialBossAnimatedAttack attack = attackGenObj.GetComponent<TutorialBossAnimatedAttack>();
        offset -= HintTime;
        offset -= SpaceTime;
        offset -= attack.WindupAnimation.length / WindupSpeed;
        offset -= attack.AttackAnimation.length / AttackSpeed / 2;
        float spawnTime = Time.time + offset;
        // Debug.Log(offset - seconds);

        while (Time.time < spawnTime)
            yield return null;

        GameObject curGenObj;
        TutorialBossCommonSwordAttackChargedgenerator curGen;
        curGenObj = InstantiateGenerator(attackGenObj);
        // childObjects.Add(curGenObj);
        curGen = curGenObj.GetComponent<TutorialBossCommonSwordAttackChargedgenerator>();
        curGen.WindupAnimationPlaySpeed = WindupSpeed;
        curGen.AttackAnimationPlaySpeed = AttackSpeed;
        curGen.soundHintTime = HintTime;
        curGen.soundSpaceTime = SpaceTime;
        curGen.windupTime = windupTime;
        curGen.parriedPitch = parriedPitch;

        // Debug.Log(retObj);
        if (retObj != null)
            retObj.Val = curGenObj;
        while (curGenObj != null)
            yield return null;
    }
    protected class DataWrapper<T>
    {
        public T Val { get; set; }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        foreach (GameObject obj in childObjects)
        {
            Destroy(obj);
        }
    }
}
