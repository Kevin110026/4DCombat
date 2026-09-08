using System.Collections;
using UnityEngine;

public class PlayerAttackAttack : AttackScript
{
    protected Animator animator;
    protected GameObject blade;
    protected GameObject sword;
    protected PlayerScript playerScript;
    protected AnimatorStateInfo animatorStateInfo;
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

    protected override void DamageCharacter(GameObject character)
    {
        playerScript.GetComponent<PlayerControlScript>().cameraScript.ScreenShakeFov();
        base.DamageCharacter(character);
        CameraScript cameraScript = spawner.GetComponent<PlayerControlScript>().cameraScript;
        float f = 8 * (1 + damage * 0.02f);
        cameraScript.ScreenShakeRotSHM((Random.Range(0, 2) == 0 ? -1 : 1) * Mathf.Sqrt(damage) * 0.5f, f, 2f, f * Mathf.Lerp(0.05f, 0.12f, damage * 0.02f));

    }
}
