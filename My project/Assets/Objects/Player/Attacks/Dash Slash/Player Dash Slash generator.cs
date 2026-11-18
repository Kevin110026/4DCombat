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
        CameraScript cameraScript = Camera.allCameras[0].GetComponentInParent<CameraScript>();

        {// calculate attack direction
            Vector3 rot = spawner.transform.rotation.eulerAngles;
            rot.x = 0; rot.z = 0;
            dir = Quaternion.Euler(rot) * transform.forward;
        }

        float lastTime = Time.time;
        float T = 0.5f;
        float energyConsume = 10f;
        float energyConsumed = 0;

        void ConsumeEnergy()
        {
            energyConsumed += playerScript.ConsumeBurstEnergy(Mathf.Min(energyConsume - energyConsumed, energyConsume * (Time.time - lastTime) / T));
        }
        StartCoroutine(PlayAnimationAndLoop(Windup));
        animator.speed = 10f;

        AttackHinterscript.NewPlayer(ImplicitHintSound, T, 0.2f, 0f);
        cameraScript.FovChangeLinear(-30, T);
        yield return LoopForSeconds(T, ConsumeEnergy);
        Debug.Log(energyConsumed);
        // playerScript.ConsumeBurstEnergy(10f);
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(PlayAnimationAndLoop(Slash));
        GameObject attObj = InstantiateAttack(attPrefab);
        attObj.GetComponent<PlayerDashSlashattack>().energy = 4.5f;
        swordTrail.ActivateAll();


        cameraScript.FovChangeQuadratic(40, 0.4f);
        float endTime = Time.time + 0.4f;
        while (stopPoint == null && Time.time < endTime)
        {
            spawner.GetComponent<CharacterController>().Move(speed * Time.deltaTime * dir);
            yield return null;
        }
        if (stopPoint != null)
            spawner.transform.position = (Vector3)stopPoint;

        playerScript.canMove = true;
        playerScript.canTurn = true;

        cameraScript.FovChangeQuadratic(-10, 0.4f);
        yield return new WaitForSeconds(0.4f);

        animator.speed = 1f;
        this.ScheduleDestroy(attObj);
        Destroy(gameObject);

        playerScript.stateScript.ResetBurstEnergyCD();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        swordTrail.DeactivateAll();
    }
}
