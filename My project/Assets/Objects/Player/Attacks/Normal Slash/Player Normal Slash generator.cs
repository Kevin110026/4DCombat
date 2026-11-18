using System.Collections;
using UnityEngine;

public class PlayerNormalSlashgenerator : PlayerAttackGenerator
{
    public AudioClip slashSound;
    public AnimationClip Windup1;
    public AnimationClip Slash1;
    public AnimationClip Windup2;
    public AnimationClip Slash2;
    PlayerNormalSlashattack attack;
    CameraScript.WaveAlongObject_Obj camWave;
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
        // camWave = playerScript.cameraScript.WaveAlongObject(blade);
        yield return PlayAnimationAndLoop(windup);
        blade.GetComponent<AudioSource>().PlayOneShot(slashSound);
        attack = InstantiateAttack<PlayerNormalSlashattack>(attackBox);
        attack.energy = playerScript.ConsumeBurstEnergy(2f);
        yield return PlayAnimationAndLoop(slash);
        yield return new WaitForSeconds(0.16f);
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.ScheduleDestroy(attack);
        Destroy(camWave);
    }
}
