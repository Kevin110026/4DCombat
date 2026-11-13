using System.Collections;
using UnityEngine;

public class TutorialBossStage1ChainxACAgenerator : TutorialBossAttackGenerator
{
    [SerializeField] GameObject Left;
    [SerializeField] GameObject LeftCharged;
    [SerializeField] GameObject Right;
    [SerializeField] GameObject RightCharged;
    [SerializeField] GameObject Down;
    [SerializeField] GameObject DownCharged;
    [SerializeField] GameObject Thrust;
    [SerializeField] GameObject ThrustCharged;
    [SerializeField] float spaceTime = 0.3f;
    static readonly float[] sepTimes = { 0.65f, 0.75f, 1.0f };
    static readonly float[] hintTimes = { 0.3f, 0.5f };
    static readonly float[] speeds = { 1.6f, 1.5f, 1.2f };
    protected override IEnumerator StartAttack()
    {
        GameObject[] normal = { Left, Right, Down, Thrust };
        GameObject[] charged = { LeftCharged, RightCharged, DownCharged, ThrustCharged };
        float t = 0.0f;

        int times = Random.Range(4, 8);
        Coroutine lastAtt = null;

        for (int i = 0; i < times; i++)
        {
            int k = Random.Range(0, 3);
            // int k = 0;
            t += sepTimes[k];
            if (Random.Range(0f, 1f) <= 0.6)
            // if (Random.Range(0f, 1f) <= 0)
            {
                GameObject picked = normal[Random.Range(0, 4)];
                // GameObject picked = normal[0];
                lastAtt = StartCoroutine(HitAfterSecond(t, picked, i == 0 ? 1.5f : speeds[k], speeds[k]));
            }
            else
            {
                GameObject picked = charged[Random.Range(0, 4)];
                // GameObject picked = charged[Random.Range(0, 1)];
                int s = Random.Range(0, 2);
                // int s = Random.Range(1, 2);
                if (t < hintTimes[s] + spaceTime)
                    t = hintTimes[s] + spaceTime;
                lastAtt = StartCoroutine(HitAfterSecond(t, picked, 40f, 5f, hintTimes[s], spaceTime, hintTimes[s] + spaceTime, 1.2f));
            }
        }
        yield return lastAtt;
        Destroy(gameObject);
    }
}
