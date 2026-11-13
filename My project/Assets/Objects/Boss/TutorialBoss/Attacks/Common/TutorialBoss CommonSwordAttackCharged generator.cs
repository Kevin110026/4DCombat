using System.Collections;
using UnityEngine;

public class TutorialBossCommonSwordAttackChargedgenerator : TutorialBossAnimatedAttack
{
    public float soundHintTime = -1;
    public float soundSpaceTime = 0.2f;
    public float windupTime = 0.27f;
    public AudioClip ImplicitHintSound;

    [HideInInspector] public bool isWindup = true;
    [HideInInspector] public bool isSlashing = false;
    public float knockbackMag = 1f;
    public HintSoundPath HintPath;

    override protected void Awake()
    {
        if (WindupAnimationPlaySpeed == -1)
        {
            WindupAnimationPlaySpeed = 10;
        }
        if (AttackAnimationPlaySpeed == -1)
        {
            AttackAnimationPlaySpeed = 4;
        }
    }
    protected override void Start()
    {
        if (soundHintTime == -1f)
        {
            soundHintTime = Random.Range(0, 2) == 0 ? 0.4f : 0.6f;
            // 0.5f;
        }
        base.Start();
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
        float k = Mathf.Clamp(dist, -0 * Time.deltaTime, 0 * Time.deltaTime);
        cc.Move(k * spawner.transform.forward);
    }

    protected virtual IEnumerator Windup()
    {
        GameObject hinter = AttackHinterscript.NewPlayer(ImplicitHintSound, soundHintTime, soundSpaceTime, 0f);
        LoadHintSoundPath(hinter, HinterLocateTool.AFaceB(target, spawner, soundHintTime), HintPath, soundHintTime);
        // hinter.transform.parent = spawner.transform;
        // hinter.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(windupTime);
        animator.speed = 0f;
        animator.Play(WindupAnimation.name);
        animator.Update(0);


        // hinter.transform.parent = spawner.transform;
        while (hinter != null)
        {
            WindupLoopAction();
            yield return null;
        }
    }
    protected virtual void SlashLoopAction()
    {
        CharacterController cc = spawner.GetComponent<CharacterController>();
        float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
        float k = Mathf.Clamp(dist, -1 * Time.deltaTime, 400 * Time.deltaTime);
        cc.Move(k * spawner.transform.forward);
    }
    TutorialBossCommonSwordAttackChargedattack attack;
    protected virtual IEnumerator Attack()
    {
        animator.speed = WindupAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(WindupAnimation));

        isSlashing = true;

        attack = InstantiateAttack<TutorialBossCommonSwordAttackChargedattack>(blade);
        attack.energy = 4.5f;
        attack.knockbackMag = knockbackMag;
        tutorialBossScript.ConsumeBurstEnergy(10f);


        AudioClip slash_sound = Resources.Load<AudioClip>("dragon-studio-whoosh-motion-405445");
        AudioSource blade_audioSource = blade.GetComponent<AudioSource>();
        blade_audioSource.clip = slash_sound;
        blade_audioSource.pitch = Random.Range(0.95f, 1.05f);
        blade_audioSource.time = 0.3f;
        blade_audioSource.volume = 0.6f;
        blade_audioSource.Play();


        animator.speed = AttackAnimationPlaySpeed;
        yield return StartCoroutine(PlayAnimationAndLoop(AttackAnimation, SlashLoopAction));

        isSlashing = false;

        // yield return new WaitForSeconds(0.04f);
        Destroy(attack);

        animator.speed = 1;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(attack);
    }
}
