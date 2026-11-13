using System.Collections;
using UnityEngine;

public class TutorialBossStage1SequanceCommonAttackChargedgenerator : TutorialBossAttackGenerator
{
    public Object LeftHit;
    public Object RightHit;
    public Object DownHit;
    public Object ThrustHit;
    GameObject currentAttackGenerator;
    protected override IEnumerator StartAttack()
    {

        currentAttackGenerator = InstantiateGenerator(LeftHit);
        // while (currentAttackGenerator != null)
        //     yield return null;
        yield return new WaitForSeconds(0.6f);

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.2f * Random.Range(0, 2));

        currentAttackGenerator = InstantiateGenerator(RightHit);
        // while (currentAttackGenerator != null)
        //     yield return null;
        yield return new WaitForSeconds(0.6f);

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.2f * Random.Range(0, 2));

        currentAttackGenerator = InstantiateGenerator(DownHit);
        // while (currentAttackGenerator != null)
        //     yield return null;
        yield return new WaitForSeconds(0.6f);

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.2f * Random.Range(0, 2));

        currentAttackGenerator = InstantiateGenerator(ThrustHit);
        // while (currentAttackGenerator != null)
        //     yield return null;
        yield return new WaitForSeconds(0.6f);

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(0.2f * Random.Range(0, 2));

        while (currentAttackGenerator != null)
            yield return null;
            
        Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(currentAttackGenerator);
    }
    // protected virtual void OnDestroy()
    // {
        
    // }
}
