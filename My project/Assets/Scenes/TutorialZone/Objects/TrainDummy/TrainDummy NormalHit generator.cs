using System.Collections;
using UnityEngine;

public class TrainDummyNormalHitgenerator : TrainDummyGenerator
{
    [SerializeField] Object AttackPrefab;
    [SerializeField] AudioClip slashSound;
    [SerializeField] AnimationClip WindupAnim;
    [SerializeField] AnimationClip SlashAnim;
    [SerializeField] AnimationClip EndAnim;

    GameObject attObj;
    bool spawned = false;

    protected override IEnumerator StartAttack()
    {
        yield return PlayAnimationAndLoop(WindupAnim);

        blade.GetComponent<AudioSource>().PlayOneShot(slashSound);
        yield return PlayAnimationAndLoop(SlashAnim, GenAttObj);

        yield return PlayAnimationAndLoop(EndAnim);
        Destroy(gameObject);
    }

    void GenAttObj()
    {
        if (!spawned && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            attObj = InstantiateAttack(AttackPrefab);
            attObj.GetComponent<TrainDummyAttack>().energy = 1f;
            attObj.GetComponent<TrainDummyAttack>().knockbackMag = 0.5f;
            spawned = true;
        }
    }
}
