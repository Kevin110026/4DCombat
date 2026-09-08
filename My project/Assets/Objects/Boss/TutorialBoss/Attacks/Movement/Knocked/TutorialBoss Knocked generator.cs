using System.Collections;
using UnityEngine;

public class TutorialBossKnockedgenerator : TutorialBossAttackGenerator
{
    [SerializeField] AnimationClip KnockedAmimation;
    [SerializeField] AnimationClip RecoverAmimation;
    [SerializeField] AudioClip RecoverSound;

    protected override IEnumerator StartAttack()
    {
        yield return PlayAnimationAndLoop(KnockedAmimation);
        yield return new WaitForSeconds(2f);
        GameObject hinter = AttackHinterscript.NewPlayer(RecoverSound, 1f, 0f, 0f);
        hinter.transform.parent = spawner.transform;
        hinter.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(1f);
        yield return PlayAnimationAndLoop(RecoverAmimation);
        yield return new WaitForSeconds(0.5f);
        // tutorialBossScript.stateScript.Hp = tutorialBossScript.stateScript.HpMax;
        Destroy(gameObject);
    }
}
