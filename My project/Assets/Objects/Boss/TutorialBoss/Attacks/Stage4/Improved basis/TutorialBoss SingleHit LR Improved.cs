using System;
using System.Collections;
using UnityEngine;

public class TutorialBossSingleHitLRImproved : TutorialBossAttackGenerator
{
    [SerializeField] float distance = 5f;
    float lastAnimTime = 0f;
    protected override IEnumerator StartAttack()
    {
        while (!ShouldMove())
            yield return null;
        while (true)
        {
            AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            if (clipInfo.clip.name != GetComponent<TutorialBossAnimatedAttack>().AttackAnimation.name)
                break;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float curAnimTime = stateInfo.normalizedTime;
            Move(curAnimTime - lastAnimTime);
            lastAnimTime = curAnimTime;
            yield return null;
        }
        // Destroy(gameObject);
    }
    protected virtual void Move(float normalizedDeltaTime)
    {
        float disp = distance * normalizedDeltaTime;
        float dis = (spawner.transform.position - target.transform.position).magnitude;
        float angle = disp / dis * Mathf.Rad2Deg;
        spawner.GetComponent<CharacterController>().enabled = false;
        spawner.transform.RotateAround(target.transform.position, transform.up, -angle);
        spawner.GetComponent<CharacterController>().enabled = true;
    }
    protected virtual bool ShouldMove()
    {
        return !GetComponent<TutorialBossCommonSwordAttackgenerator>().isWindup;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Move(1 - lastAnimTime);
    }
}
