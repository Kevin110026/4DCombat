using System.Collections;
using UnityEngine;

public class TutorialBossStage1SequanceCommonAttackgenerator : TutorialBossAttackGenerator
{
    public Object LeftHit;
    public Object RightHit;
    public Object DownHit;
    public Object ThrustHit;
    GameObject currentAttackGenerator;
    protected override IEnumerator StartAttack()
    {

        currentAttackGenerator = InstantiateGenerator(LeftHit);
        currentAttackGenerator.GetComponent<TutorialBossCommonSwordAttackgenerator>().WindupStallTime = 0;
        while (currentAttackGenerator != null)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        currentAttackGenerator = InstantiateGenerator(RightHit);
        currentAttackGenerator.GetComponent<TutorialBossCommonSwordAttackgenerator>().WindupStallTime = 0;
        while (currentAttackGenerator != null)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        currentAttackGenerator = InstantiateGenerator(DownHit);
        currentAttackGenerator.GetComponent<TutorialBossCommonSwordAttackgenerator>().WindupStallTime = 0;
        while (currentAttackGenerator != null)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        currentAttackGenerator = InstantiateGenerator(ThrustHit);
        currentAttackGenerator.GetComponent<TutorialBossCommonSwordAttackgenerator>().WindupStallTime = 0;
        while (currentAttackGenerator != null)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(currentAttackGenerator);
    }
    // protected virtual void OnDestroy()
    // {
    //     Destroy(currentAttackGenerator);
    // }
}
