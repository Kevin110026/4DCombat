using UnityEngine;

public class TrainDummyGenerator : AttackGenerator
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected TrainDummyScript trainDummyScript;
    // protected AnimatorStateInfo animatorStateInfo;
    protected override void Start()
    {
        animator = spawner.GetComponent<Animator>();
        trainDummyScript = spawner.GetComponent<TrainDummyScript>();
        sword = trainDummyScript.sword;
        blade = trainDummyScript.blade;
        base.Start();
    }
}
