using UnityEngine;

public class TutorialBossAttackGenerator : AttackGenerator
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected TutorialBossScript tutorialBossScript;
    // protected AnimatorStateInfo animatorStateInfo;
    protected override void Start()
    {
        animator = spawner.GetComponent<Animator>();
        tutorialBossScript = spawner.GetComponent<TutorialBossScript>();
        sword = tutorialBossScript.sword;
        blade = tutorialBossScript.blade;
        base.Start();
    }
}
