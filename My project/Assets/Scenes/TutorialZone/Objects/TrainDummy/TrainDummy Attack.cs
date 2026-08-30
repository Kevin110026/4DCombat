using UnityEngine;

public class TrainDummyAttack : AttackScript
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected TrainDummyScript trainDummyScript;
    protected override void Start()
    {
        base.Start();
        animator = spawner.GetComponent<Animator>();
        trainDummyScript = spawner.GetComponent<TrainDummyScript>();
        blade = trainDummyScript.blade;
        sword = trainDummyScript.sword;
    }
}
