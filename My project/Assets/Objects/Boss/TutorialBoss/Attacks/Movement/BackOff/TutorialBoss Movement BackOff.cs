using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialBossMovementBackOff : TutorialBossAttackGenerator
{

    public float BackOffDistance = 10;
    public float BackOffAcceleration = 80;
    protected override IEnumerator StartAttack()
    {
        animator.Play("TutorialBoss Idle");
        tutorialBossScript.FaceTarget();
        float dist = BackOffDistance;
        Vector3 ori = spawner.transform.position;
        Vector3 dest = spawner.transform.position - dist * spawner.transform.forward;
        if (Physics.Raycast(spawner.transform.position, -spawner.transform.forward, dist, LayerMask.GetMask("Wall")))
        {
            // Debug.Log("Yes");
            do
            {
                dest = new Vector3(UnityEngine.Random.Range(-19f, 19f), spawner.transform.position.y, UnityEngine.Random.Range(-19f, 19f));
            } while ((dest - target.transform.position).magnitude < BackOffDistance);
        }

        Vector3 dir = (dest - ori).normalized;
        dist = (ori - dest).magnitude;
        float speed = 0;
        while (true)
        {
            float d = (ori - spawner.transform.position).magnitude;
            if (d >= dist - 0.1f) break;
            speed += (d < dist / 2 ? 1 : -1) * Time.deltaTime * BackOffAcceleration;
            speed = Mathf.Max(speed, BackOffAcceleration * 0.1f);
            spawner.GetComponent<CharacterController>()
            .Move(Mathf.Min(Time.deltaTime * speed, dist - d) * dir);
            tutorialBossScript.FaceTarget();
            yield return null;
        }
        Destroy(gameObject);
    }
}
