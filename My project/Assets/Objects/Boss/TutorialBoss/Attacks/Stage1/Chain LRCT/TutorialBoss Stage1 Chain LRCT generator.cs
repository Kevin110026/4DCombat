using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBossStage1ChainLRCTgenerator : TutorialBossAttackGenerator
{
    public GameObject LeftHit;
    public GameObject RightHit;
    public GameObject ThrustHitCharged;

    private static readonly float[][] hitTimes = new float[][] {
        new float[] { 0.5f, 1.0f },
        new float[] { 1.0f, 0.5f }
        };

    protected override IEnumerator StartAttack()
    {
        int type = Random.Range(0, 2);
        float hitTime = 0.5f;
        StartCoroutine(HitAfterSecond(hitTime, LeftHit, 1.7f, 2.0f));
        hitTime += hitTimes[type][0];
        StartCoroutine(HitAfterSecond(hitTime, RightHit, 1.2f, 2.0f));
        hitTime += hitTimes[type][1];
        yield return StartCoroutine(HitAfterSecond(hitTime, ThrustHitCharged, 5f, 10f, 0.3f, 0.3f, 0.27f));

        Destroy(gameObject);
    }
}
