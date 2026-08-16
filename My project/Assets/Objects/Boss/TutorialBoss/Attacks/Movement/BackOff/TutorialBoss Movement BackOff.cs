using System.Collections;
using UnityEngine;

public class TutorialBossMovementBackOff : TutorialBossAttackGenerator
{
    protected override IEnumerator StartAttack()
    {
        animator.Play("TutorialBoss Idle");
        tutorialBossScript.FaceTarget();
        float dist = 5f;
        Vector3 ori = spawner.transform.position;
        Vector3 dest = spawner.transform.position - dist * spawner.transform.forward;
        if (Physics.Raycast(spawner.transform.position, -spawner.transform.forward, dist, LayerMask.GetMask("Wall")))
        {
            // Debug.Log("Yes");
            do
            {
                dest = new Vector3(UnityEngine.Random.Range(-14f, 14f), spawner.transform.position.y, UnityEngine.Random.Range(-14f, 14f));
            } while ((dest - target.transform.position).magnitude < 5f);
        }

        Vector3 dir = (dest - ori).normalized;
        dist = (ori - dest).magnitude;
        float speed = 0;
        float accelerate = 40f;
        while (true)
        {
            float d = (ori - spawner.transform.position).magnitude;
            if (d >= dist - 0.1f) break;
            speed += (d < dist / 2 ? 1 : -1) * Time.deltaTime * accelerate;
            speed = Mathf.Max(speed, accelerate * 0.1f);
            spawner.GetComponent<CharacterController>()
            .Move(Mathf.Min(Time.deltaTime * speed, dist - d) * dir);
            tutorialBossScript.FaceTarget();
            yield return null;
        }
        Destroy(gameObject);
    }
}
