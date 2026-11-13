using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class TutorialBossSingleHitDownImprovedgenerator : TutorialBossCommonSwordAttackgenerator
{
    int windupLoopMode = -1;
    float lastK = 0;
    [SerializeField] float airMoveSpeed = 9f;
    [SerializeField] float maxAirTurn = 135f;
    [SerializeField] float groundMoveSpeed = 18f;
    protected override void WindupLoopAction()
    {
        if (windupLoopMode == -1)
        {
            Vector3 deltaPos = spawner.transform.position - target.transform.position;
            deltaPos.y = 0;
            float dist = deltaPos.magnitude;
            Debug.Log(dist);
            if (dist <= 5)
            {
                windupLoopMode = 0;
                spawner.GetComponent<CharacterController>().Move(Vector3.down * 0.1f);
                spawner.GetComponent<CharacterMovementScript>().TryJump(2f);
            }
            else if (dist <= 9f)
            {
                windupLoopMode = 1;
                spawner.GetComponent<CharacterController>().Move(Vector3.down * 0.1f);
                spawner.GetComponent<CharacterMovementScript>().TryJump(2f);
            }
            else
            {
                windupLoopMode = 2;
            }
        }

        if (windupLoopMode == 0 || windupLoopMode == 1)
        {
            float airMoveSpeedScale;
            float airTurnSpeedScale;
            if (windupLoopMode == 0)
            {
                airMoveSpeedScale = 1f / 3f;
                airTurnSpeedScale = 1f;
            }
            else
            {
                airMoveSpeedScale = 1f;
                airTurnSpeedScale = 1f / 3f;
            }

            // tutorialBossScript.FaceTarget();
            AnimatorClipInfo asi = animator.GetCurrentAnimatorClipInfo(0)[0];
            if (asi.clip != WindupAnimation)
                return;
            float animProg = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Vector3 deltaPos = target.transform.position - spawner.transform.position;
            deltaPos.y = 0;
            Quaternion tarRot = Quaternion.RotateTowards(spawner.transform.rotation, Quaternion.LookRotation(deltaPos), (animProg - lastK) * airTurnSpeedScale * maxAirTurn);
            spawner.transform.rotation = tarRot;
            lastK = animProg;
            float deltaDist = deltaPos.magnitude;
            float moveDist = Mathf.Clamp(deltaDist, -airMoveSpeedScale * airMoveSpeed * Time.deltaTime, airMoveSpeedScale * airMoveSpeed * Time.deltaTime);
            spawner.GetComponent<CharacterController>().Move(spawner.transform.forward * moveDist);
        }
        else if (windupLoopMode == 2)
        {
            CharacterController cc = spawner.GetComponent<CharacterController>();
            float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
            float k = Mathf.Clamp(dist, -0.25f * groundMoveSpeed * Time.deltaTime, 0.5f * groundMoveSpeed * Time.deltaTime);
            cc.Move(k * spawner.transform.forward);
        }
    }

    bool attackLoopDowned = false;
    protected override void AttackLoopAction()
    {
        if (!attackLoopDowned)
        {
            attackLoopDowned = true;
        }
        spawner.GetComponent<CharacterMovementScript>().AddVelocity(animator.speed * Time.deltaTime * Vector3.down * 20f);
        base.AttackLoopAction();
        Vector3 deltaPos = target.transform.position - spawner.transform.position;
        deltaPos.y = 0;
        Quaternion tarRot = Quaternion.RotateTowards(spawner.transform.rotation, Quaternion.LookRotation(deltaPos), Time.deltaTime * 30f);
    }
}
