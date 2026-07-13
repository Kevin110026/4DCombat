using System.Collections;
using UnityEngine;

public class TutorialBossStage1SequanceCommonAttackChargedgenerator : TutorialBossAttackGenerator
{
    public Object LeftHit;
    public Object RightHit;
    public Object DownHit;
    public Object ThrustHit;
    protected override IEnumerator StartAttack()
    {
        GameObject currentAttackGenerator;

        currentAttackGenerator = InstantiateGenerator(LeftHit);
        while (currentAttackGenerator != null)
            yield return null;

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(Random.Range(0.0f, 0.6f));

        currentAttackGenerator = InstantiateGenerator(RightHit);
        while (currentAttackGenerator != null)
            yield return null;

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(Random.Range(0.0f, 0.6f));

        currentAttackGenerator = InstantiateGenerator(DownHit);
        while (currentAttackGenerator != null)
            yield return null;

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(Random.Range(0.0f, 0.6f));

        currentAttackGenerator = InstantiateGenerator(ThrustHit);
        while (currentAttackGenerator != null)
            yield return null;

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(Random.Range(0.0f, 0.6f));

        Destroy(gameObject);
    }
}
