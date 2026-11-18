using System.Collections;
using UnityEngine;

public class TutorialBossWhirlwindSpinBegin : TutorialBossAnimatedAttack
{
    [SerializeField] AnimationClip SpinBeginAnimation;
    [HideInInspector] public float knockbackMag = 1f;
    protected override IEnumerator StartAttack()
    {
        TutorialBossWhirlwindattack attack = InstantiateAttack<TutorialBossWhirlwindattack>(blade);
        attack.spawner = spawner;
        attack.target = target;
        attack.energy = attack.BaseEnergy;
        attack.knockbackMag = knockbackMag;
        tutorialBossScript.ConsumeBurstEnergy(attack.EnergyCost);

        yield return StartCoroutine(PlayAnimationAndLoop(SpinBeginAnimation));
        Destroy(gameObject);
    }
}
