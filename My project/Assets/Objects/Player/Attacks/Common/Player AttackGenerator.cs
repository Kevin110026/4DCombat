using UnityEngine;

public class PlayerAttackGenerator : AttackGenerator
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected PlayerScript playerScript;
    // protected AnimatorStateInfo animatorStateInfo;
    protected GameObject attackBox;
    protected override void Start()
    {
        animator = spawner.GetComponent<Animator>();
        playerScript = spawner.GetComponent<PlayerScript>();
        sword = playerScript.sword;
        blade = playerScript.blade;
        attackBox = playerScript.attackBox;
        base.Start();
    }
}
