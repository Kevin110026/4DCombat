using System.Collections;
using UnityEngine;

public class TutorialBossWhirlwindSpinning : TutorialBossAnimatedAttack
{
    public float duration = 5f;

    protected override IEnumerator StartAttack()
    {
        TutorialBossWhirlwindattack attack = blade.GetComponent<TutorialBossWhirlwindattack>();
        CharacterScript cs = spawner.GetComponent<CharacterScript>();
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            if (!attack.canDamage && attack.CDEnd())// && cs.stateScript.BurstEnergy >= attack.EnergyCost)
            {
                attack.RefreshAttack();
            }
            spawner.transform.Rotate(new Vector3(0, -360 * 2 * Time.deltaTime, 0));
            yield return null;
        }
        Destroy(gameObject);
    }
}
