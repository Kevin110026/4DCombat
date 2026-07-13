using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialBossWhirlwindgenerator : TutorialBossAnimatedAttack
{
    [SerializeField] Object Windup;
    [SerializeField] Object SpinBegin;
    [SerializeField] Object Spinning;
    [SerializeField] Object SpinEnd;
    [SerializeField] float RotationSpeed = 90f;
    [SerializeField] float MoveSpeed = 3f;
    private bool move = false;
    private Quaternion rot;
    protected override void Update()
    {
        base.Update();
        if (move)
        {
            Vector3 face = target.transform.position - spawner.transform.position;
            face.y = 0;
            Quaternion tar = Quaternion.LookRotation(face);
            rot = Quaternion.RotateTowards(rot, tar, RotationSpeed * Time.deltaTime);
            spawner.GetComponent<CharacterController>().Move(rot * Vector3.forward * MoveSpeed * Time.deltaTime);
        }
    }
    protected override IEnumerator StartAttack()
    {
        GameObject windup = InstantiateGenerator(Windup);
        while (windup != null)
            yield return null;
        move = true;

        Vector3 face = target.transform.position - spawner.transform.position;
        face.y = 0;
        rot = Quaternion.LookRotation(face);
        SwordTrail swordTrail = sword.transform.Find("trail").GetComponent<SwordTrail>();
        swordTrail.ActivateNormal();

        GameObject spinBegin = InstantiateGenerator(SpinBegin);
        while (spinBegin != null)
            yield return null;
        GameObject spinning = InstantiateGenerator(Spinning);
        while (spinning != null)
            yield return null;
        GameObject spinEnd = InstantiateGenerator(SpinEnd);
        while (spinEnd != null)
            yield return null;
        Destroy(gameObject);
    }
}
