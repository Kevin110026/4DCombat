using System.Collections;
using UnityEngine;

public class TutorialBossMovementIdle : TutorialBossAttackGenerator
{
    protected override IEnumerator StartAttack()
    {
        animator.Play("TutorialBoss Idle");
        float forwardSpeed = 0;
        float rightSpeed = 0;
        float leftright = Random.Range(0, 2) == 0 ? 1f : -1f;
        float leftrightSwapCd = 0;
        while (true)
        {
            tutorialBossScript.FaceTarget();
            leftrightSwapCd -= Time.deltaTime;
            if (leftrightSwapCd <= 0f)
            {
                leftrightSwapCd += Random.Range(2f, 4f);
                leftright = -leftright;
            }
            rightSpeed = Mathf.Clamp(rightSpeed + Time.deltaTime * leftright * 9, -6f, 6f);

            float dist = (spawner.transform.position - target.transform.position).magnitude;
            if (dist > 6)
            {
                forwardSpeed = Mathf.Clamp(forwardSpeed + 16 * Time.deltaTime, -8, 16);
                rightSpeed *= 0.3f;
            }
            else if (dist < 2.5)
            {
                forwardSpeed = Mathf.Clamp(forwardSpeed - 16 * Time.deltaTime, -8, 16);
                rightSpeed *= 0.3f;
            }
            else
                forwardSpeed = Mathf.Clamp(forwardSpeed - 16 * Mathf.Sign(forwardSpeed) * Time.deltaTime, -8, 16);

            spawner.GetComponent<CharacterController>()
            .Move(Time.deltaTime * (spawner.transform.forward * forwardSpeed + spawner.transform.right * rightSpeed));

            yield return null;
        }
    }
}
