using UnityEngine;
using UnityEngine.Analytics;

public class TutorialBossSingleHitThrustImprovedgenerator : TutorialBossStage1SingleHitThrustgenerator
{
    [SerializeField] float maxFloatDistanceX = 8f;
    [SerializeField] float maxFloatSpeedX = 16f;
    float floatDistanceX;
    [SerializeField] float maxFloatDistanceZ = 5f;
    [SerializeField] float maxFloatSpeedZ = 100f;
    float floatDistanceZ;
    [SerializeField] float maxThrustSpeed = 24f;
    int windupLoopMode = -1;
    float lastK = 0;
    protected override void WindupLoopAction()
    {
        if (windupLoopMode == -1)
        {
            // Debug.Log((spawner.transform.position - target.transform.position).magnitude);
            if ((spawner.transform.position - target.transform.position).magnitude <= 8f)
            {
                // Debug.Log(0);
                windupLoopMode = 0;
                floatDistanceX = Random.Range(-maxFloatDistanceX, maxFloatDistanceX);
                floatDistanceZ = Random.Range(-maxFloatDistanceZ, maxFloatDistanceZ);

            }
            else
            {
                // Debug.Log(1);
                windupLoopMode = 1;
            }
        }

        tutorialBossScript.FaceTarget();
        if (windupLoopMode == 0)
        {
            AnimatorClipInfo asi = animator.GetCurrentAnimatorClipInfo(0)[0];
            // Debug.Log("hi");
            if (asi.clip != WindupAnimation)
                return;
            // Debug.Log("hi2");
            float animProg = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            tutorialBossScript.FaceTarget();
            Vector3 moveVec = new Vector3(floatDistanceX, 0, floatDistanceZ) * (animProg - lastK);
            moveVec.x = Mathf.Min(maxFloatSpeedX * Time.deltaTime, moveVec.x);
            moveVec.z = Mathf.Min(maxFloatSpeedZ * Time.deltaTime, moveVec.z);
            lastK = animProg;
            spawner.GetComponent<CharacterController>().Move(spawner.transform.rotation * moveVec);
        }
        else if (windupLoopMode == 1)
        {
            CharacterController cc = spawner.GetComponent<CharacterController>();
            float dist = (spawner.transform.position - target.transform.position).magnitude - 3f;
            float k = Mathf.Clamp(dist, -0.25f * maxThrustSpeed * Time.deltaTime, 0.5f * maxThrustSpeed * Time.deltaTime);
            cc.Move(k * spawner.transform.forward);
        }
    }
}
