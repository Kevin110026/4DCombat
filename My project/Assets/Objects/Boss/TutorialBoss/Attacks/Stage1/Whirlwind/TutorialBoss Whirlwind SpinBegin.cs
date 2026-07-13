using System.Collections;
using UnityEngine;

public class TutorialBossWhirlwindSpinBegin : TutorialBossAnimatedAttack
{
    [SerializeField] AnimationClip SpinBeginAnimation;
    protected override IEnumerator StartAttack()
    {
        TutorialBossWhirlwindattack attack = blade.AddComponent<TutorialBossWhirlwindattack>();
        attack.spawner = spawner;
        attack.target = target;
        attack.energy = attack.BaseEnergy;
        tutorialBossScript.ConsumeBurstEnergy(attack.EnergyCost);

        yield return StartCoroutine(PlayAnimationAndLoop(SpinBeginAnimation));
        Destroy(gameObject);
    }
}
