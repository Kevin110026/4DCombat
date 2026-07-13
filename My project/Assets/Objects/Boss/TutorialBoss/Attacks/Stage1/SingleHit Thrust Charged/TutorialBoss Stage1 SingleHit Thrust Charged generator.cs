using UnityEngine;

public class TutorialBossStage1SingleHitThrustChargedgenerator : TutorialBossCommonSwordAttackgenerator
{
    protected override void AttackLoopAction()
    {
        CharacterController cc = spawner.GetComponent<CharacterController>();
        float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
        float maxd = 25f * Time.deltaTime;
        float moved = Mathf.Clamp(dist, -maxd, maxd);
        cc.Move(moved * spawner.transform.forward);
    }
}
