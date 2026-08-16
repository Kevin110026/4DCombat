using System.Collections;
using UnityEngine;

public class PlayerNormalSlashgenerator : PlayerAttackGenerator
{
    public AudioClip slashSound;
    public AnimationClip Windup1;
    public AnimationClip Slash1;
    public AnimationClip Windup2;
    public AnimationClip Slash2;

    protected override IEnumerator StartAttack()
    {
        AnimationClip windup;
        AnimationClip slash;
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animatorStateInfo.IsName("Player Normal Slash 1"))
        {
            windup = Windup1;
            slash = Slash1;
        }
        else
        {
            windup = Windup2;
            slash = Slash2;
        }
        yield return PlayAnimationAndLoop(windup);
        blade.GetComponent<AudioSource>().PlayOneShot(slashSound);
        PlayerNormalSlashattack attack = InstantiateAttack<PlayerNormalSlashattack>(attackBox);
        attack.energy = 2f;
        yield return PlayAnimationAndLoop(slash);
        yield return new WaitForSeconds(0.16f);
        Destroy(attack);
        Destroy(gameObject);
    }
}
