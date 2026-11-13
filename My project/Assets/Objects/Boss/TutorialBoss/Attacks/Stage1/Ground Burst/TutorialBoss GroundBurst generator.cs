using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBossGroundBurstgenerator : TutorialBossAttackGenerator
{
    public float windupTime = 2f;
    public AudioClip ImplicitHintSound;

    public float slashWindow = 0.3f;
    public float waveNormalizedSpeed = 20f;
    public int waveNormalizedLength = 20;
    [SerializeField] Object attackPerfab;
    [SerializeField] HintSoundPath hintPath;
    [HideInInspector] public bool attackedPlayerThisTick = false;

    [SerializeField] AnimationClip WindupAnim;
    [SerializeField] AnimationClip SlashAnim;

    protected override IEnumerator StartAttack()
    {
        yield return StartCoroutine(Windup());
        yield return StartCoroutine(Slash());
        Destroy(gameObject);
    }

    IEnumerator Windup()
    {
        tutorialBossScript.ConsumeBurstEnergy(30f);
        yield return PlayAnimationAndLoop(WindupAnim);
        GameObject hinter = AttackHinterscript.NewPlayer(ImplicitHintSound, windupTime, slashWindow, 0f);
        LoadHintSoundPath(hinter, HinterLocateTool.AFaceB(target, spawner), hintPath, windupTime);
        // hinter.transform.parent = spawner.transform;
        // hinter.transform.localPosition = Vector3.zero;
        hinter.GetComponent<AudioSource>().pitch = 0.3f;
        yield return new WaitForSeconds(windupTime);
    }

    Vector3 spawnDir;
    Vector3 spwanCenter;
    IEnumerator Slash()
    {
        tutorialBossScript.FaceTarget();
        spawner.transform.Rotate(new Vector3(0, Random.Range(-10, 10), 0));
        spawnDir = spawner.transform.forward;
        spwanCenter = spawner.transform.position;
        yield return new WaitForSeconds(slashWindow);
        
        StartCoroutine(PlayAnimationAndLoop(SlashAnim));

        float slashTime = Time.time;
        for (int i = 0; i < waveNormalizedLength; i++)
        {
            float dist = i + 0.5f;
            while ((Time.time - slashTime) * waveNormalizedSpeed < dist)
                yield return null;
            spawnAttackWave(dist, 0);
            if (i <= 2)
            {
                spawnAttackWave(dist, 45f);
                spawnAttackWave(dist, -45f);
                spawnAttackWave(dist, 22.5f);
                spawnAttackWave(dist, -22.5f);
            }
        }

        yield return new WaitForSeconds(1.5f);
    }

    GameObject spawnAttackWave(float normalizedDistance = 0, float rot = 0)
    {
        GameObject attObj;
        attObj = InstantiateAttack(attackPerfab);
        float sep = attObj.transform.lossyScale.z;
        Vector3 pos = spawner.transform.position;
        pos.y = 0;
        attObj.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(spawnDir));
        attObj.transform.Rotate(new Vector3(0, rot, 0));
        attObj.transform.position += normalizedDistance * sep * attObj.transform.forward;
        return attObj;
    }


    protected override void Update()
    {
        base.Update();
        attackedPlayerThisTick = false;
    }

}
