using System.Collections;
using Mono.Cecil;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TutorialBossCommonSwordAttackgenerator : TutorialBossAnimatedAttack
{
    public float WindupStallTime = 0;
    protected bool isWindup = true;

    protected virtual void Awake()
    {
        if (WindupAnimationPlaySpeed == -1)
        {
            WindupAnimationPlaySpeed = 1;
        }
        if (AttackAnimationPlaySpeed == -1)
        {
            AttackAnimationPlaySpeed = 1;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (WindupStallTime == -1)
        {
            WindupStallTime = Random.Range(0.3f, 0.5f);
        }
    }

    protected override IEnumerator StartAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isWindup = true;
        yield return StartCoroutine(Windup());
        isWindup = false;
        yield return StartCoroutine(Attack());

        Destroy(gameObject);
    }
    protected virtual void WindupLoopAction()
    {
        tutorialBossScript.FaceTarget();
        CharacterController cc = spawner.GetComponent<CharacterController>();
        float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
        float k = Mathf.Clamp(dist, -4 * Time.deltaTime, 8 * Time.deltaTime);
        cc.Move(k * spawner.transform.forward);
    }

    protected virtual IEnumerator Windup()
    {
        animator.speed = 0;
        animator.Play(WindupAnimation.name);
        animator.Update(0);
        yield return new WaitForSeconds(WindupStallTime);
        animator.speed = WindupAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(WindupAnimation, WindupLoopAction));
    }

    protected virtual void AttackLoopAction()
    {
        CharacterController cc = spawner.GetComponent<CharacterController>();
        float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
        float k = Mathf.Clamp(dist, -1 * Time.deltaTime, 4 * Time.deltaTime);
        cc.Move(k * spawner.transform.forward);
    }

    protected virtual IEnumerator Attack()
    {

        TutorialBossCommonSwordAttackattack attack = InstantiateAttack<TutorialBossCommonSwordAttackattack>(blade);
        attack.energy = 4.5f;
        tutorialBossScript.ConsumeBurstEnergy(4.5f);

        AudioClip slash_sound = Resources.Load<AudioClip>("dragon-studio-whoosh-motion-405445");
        AudioSource blade_audioSource = blade.GetComponent<AudioSource>();
        blade_audioSource.clip = slash_sound;
        blade_audioSource.pitch = Random.Range(0.95f, 1.05f);
        blade_audioSource.time = 0.3f;
        blade_audioSource.volume = 0.6f;
        blade_audioSource.Play();


        animator.speed = AttackAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(AttackAnimation, AttackLoopAction));

        Destroy(attack);
    }
}
