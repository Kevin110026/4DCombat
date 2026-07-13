using UnityEngine;

public class TutorialBossAttackAttack : AttackScript
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected TutorialBossScript tutorialBossScript;
    protected override void Start()
    {
        base.Start();
        animator = spawner.GetComponent<Animator>();
        tutorialBossScript = spawner.GetComponent<TutorialBossScript>();
        blade = tutorialBossScript.blade;
        sword = tutorialBossScript.sword;
    }
}
