using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class TutorialBossStage1SingleHitThrustgenerator : TutorialBossCommonSwordAttackgenerator
{
    [SerializeField] Object attPerfab;
    CharacterController tbscc;
    protected override void Start()
    {
        base.Start();
        tbscc = spawner.GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        base.Update();
        if (!isWindup)
        {
            float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
            float maxd = 15f * Time.deltaTime;
            float moved = Mathf.Clamp(dist, -maxd, maxd);
            tbscc.Move(moved * spawner.transform.forward);
        }
    }
    protected override IEnumerator Attack()
    {
        GameObject attObj = InstantiateAttack(attPerfab);
        attObj.GetComponent<TutorialBossStage1SingleHitThrustattack>().energy = 4.5f;
        attObj.GetComponent<TutorialBossStage1SingleHitThrustattack>().knockbackMag = knockbackMag;
        tutorialBossScript.ConsumeBurstEnergy(4.5f);

        
        TutorialBossCommonSwordAttackattack fakeAtt = InstantiateAttack<TutorialBossCommonSwordAttackattack>(blade);
        fakeAtt.energy = 0;
        fakeAtt.canDamage = false;
        fakeAtt.canBeParried = false;

        AudioClip slash_sound = Resources.Load<AudioClip>("dragon-studio-whoosh-motion-405445");
        AudioSource blade_audioSource = blade.GetComponent<AudioSource>();
        blade_audioSource.clip = slash_sound;
        blade_audioSource.pitch = Random.Range(0.95f, 1.05f);
        blade_audioSource.time = 0.3f;
        blade_audioSource.volume = 0.6f;
        blade_audioSource.Play();


        animator.speed = AttackAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(AttackAnimation, AttackLoopAction));

        Destroy(attObj);
        Destroy(fakeAtt);
    }
}
