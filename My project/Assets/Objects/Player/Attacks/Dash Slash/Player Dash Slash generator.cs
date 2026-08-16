using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDashSlashgenerator : PlayerAttackGenerator
{
    [SerializeField] Object attPrefab;
    [SerializeField] AnimationClip Windup;
    [SerializeField] AnimationClip Slash;
    [SerializeField] AudioClip ImplicitHintSound;
    public float speed = 3600f;
    [HideInInspector] public Vector3? stopPoint = null;
    [HideInInspector] public Vector3 dir;

    
    private SwordTrail swordTrail;

    protected override void Start()
    {
        base.Start();
        
        swordTrail = sword.transform.Find("trail").GetComponent<SwordTrail>();
    }

    protected override IEnumerator StartAttack()
    {
        playerScript.canMove = false;
        playerScript.canTurn = false;

        {// calculate attack direction
            Vector3 rot = spawner.transform.rotation.eulerAngles;
            rot.x = 0; rot.z = 0;
            dir = Quaternion.Euler(rot) * transform.forward;
        }

        playerScript.ConsumeBurstEnergy(10f);
        StartCoroutine(PlayAnimationAndLoop(Windup));
        animator.speed = 10f;

        AttackHinterscript.NewPlayer(ImplicitHintSound, 0.5f, 0.2f, 0f);
        Camera cam = spawner.GetComponent<PlayerControlScript>().playerCamera;
        float endTime = Time.time + 0.7f;
        float oriFov = cam.fieldOfView;
        float deltaFov = 30;
        while (Time.time < endTime)
        {
            cam.fieldOfView = Mathf.Max(oriFov - deltaFov, cam.fieldOfView - Time.deltaTime * deltaFov / 0.5f);
            yield return null;
        }

        StartCoroutine(PlayAnimationAndLoop(Slash));
        GameObject attObj = InstantiateAttack(attPrefab);
        attObj.GetComponent<PlayerDashSlashattack>().energy = 4.5f;
        swordTrail.ActivateAll();

        endTime = Time.time + 0.4f;
        deltaFov = 20f;
        while (stopPoint == null && Time.time < endTime)
        {
            spawner.GetComponent<CharacterController>().Move(speed * Time.deltaTime * dir);
            cam.fieldOfView = (oriFov + deltaFov) + (cam.fieldOfView - oriFov - deltaFov) * Mathf.Pow(0.03f, Time.deltaTime);
            yield return null;
        }
        if (stopPoint != null)
            spawner.transform.position = (Vector3)stopPoint;

        playerScript.canMove = true;
        playerScript.canTurn = true;

        endTime = Time.time + 0.4f;
        deltaFov = 0;
        while (Time.time < endTime)
        {
            cam.fieldOfView = (oriFov + deltaFov) + (cam.fieldOfView - oriFov - deltaFov) * Mathf.Pow(0.02f, Time.deltaTime);
            yield return null;
        }

        animator.speed = 1f;
        cam.fieldOfView = oriFov;
        Destroy(attObj);
        Destroy(gameObject);

        // playerScript.
    }

    protected virtual void OnDestroy()
    {
        swordTrail.DeactivateAll();
    }
}
