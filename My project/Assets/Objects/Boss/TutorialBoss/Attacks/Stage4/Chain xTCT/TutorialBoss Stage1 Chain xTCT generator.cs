using System.Collections;
using UnityEngine;

public class TutorialBossStage1ChainxTCTgenerator : TutorialBossAttackGenerator
{
    [SerializeField] GameObject Thrust;
    [SerializeField] GameObject ThrustCharged;
    [SerializeField] float spaceTime = 0.27f;
    static readonly float[] sepTimes = { 0.4f, 0.4f, 0.7f };
    static readonly float[] hintTimes = { 0.2f, 0.4f };
    static readonly float[] speeds = { 1.6f, 1.6f, 1.4f };
    protected override IEnumerator StartAttack()
    {
        float t = 0.0f;

        int times = Random.Range(3, 6);
        Coroutine lastAtt = null;

        bool chargedLastTime = true;
        for (int i = 0; i < times; i++)
        {
            int k = Random.Range(0, 3);
            t += sepTimes[k];
            if (i == times - 2 || chargedLastTime || (i < times - 2 && Random.Range(0f, 1f) <= 0.65) || (i == times - 1 && Random.Range(0f, 1f) <= 0.1))
            {
                if (i == 0 && k == 0)
                    t += 0.1f;
                chargedLastTime = false;
                lastAtt = StartCoroutine(HitAfterSecond(t, Thrust, i == 0 ? 1.5f : speeds[k], 1.35f * speeds[k]));
            }
            else
            {
                int s = Random.Range(0, 2);
                if (t < hintTimes[s] + spaceTime)
                    t = hintTimes[s] + spaceTime;
                chargedLastTime = true;
                lastAtt = StartCoroutine(HitAfterSecond(t, ThrustCharged, i == 0 ? 1.5f : 20f, 20f, hintTimes[s], spaceTime, hintTimes[s] + spaceTime, 1.2f));
            }
        }
        yield return lastAtt;
        Destroy(gameObject);
    }
}
