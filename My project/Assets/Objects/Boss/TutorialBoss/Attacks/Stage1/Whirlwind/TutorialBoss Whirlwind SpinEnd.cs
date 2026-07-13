using System.Collections;
using UnityEngine;

public class TutorialBossWhirlwindSpinEnd : TutorialBossAnimatedAttack
{
    [SerializeField] AnimationClip SpinEndAnimation;
    protected override IEnumerator StartAttack()
    {
        yield return StartCoroutine(PlayAnimationAndLoop(SpinEndAnimation));
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        TutorialBossWhirlwindattack attack = blade.GetComponent<TutorialBossWhirlwindattack>();
        if (attack != null)
            Destroy(attack);
    }
}
