using UnityEngine;

public class TutorialBossStage1SingleHitThrustgenerator : TutorialBossCommonSwordAttackgenerator
{
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
}
