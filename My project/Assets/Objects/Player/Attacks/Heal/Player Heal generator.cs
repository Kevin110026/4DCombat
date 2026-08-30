using System;
using System.Collections;
using UnityEngine;

public class PlayerHealgenerator : PlayerAttackGenerator
{
    [SerializeField] AudioClip HealChargingSound;
    [SerializeField] AudioClip HealSuccessSound;
    [SerializeField] float energyCost = 18f;
    [SerializeField] float heal = 50f;

    [SerializeField] float healTimeFront = 1f;
    [SerializeField] float healTimeBack = 0.3f;

    protected override IEnumerator StartAttack()
    {
        playerScript.canMove = false;

        float energyConsumed = 0;

        AttackHinterscript.NewPlayer(HealChargingSound, healTimeFront - 0.1f, 0.1f, 0f);

        Camera cam = spawner.GetComponent<PlayerControlScript>().playerCamera;
        float endTime = Time.time + healTimeFront;
        float oriFov = cam.fieldOfView;
        float deltaFov = 12;
        while (Time.time < endTime)
        {
            cam.fieldOfView = Mathf.Max(oriFov - deltaFov, cam.fieldOfView - Time.deltaTime * deltaFov / (healTimeFront - 0.1f));
            float eDiff = (Time.time - endTime + healTimeFront) / healTimeFront * energyCost - energyConsumed;
            playerScript.ConsumeBurstEnergy(eDiff);
            energyConsumed += eDiff;
            yield return null;
        }
        playerScript.ConsumeBurstEnergy(energyCost - energyConsumed);
        playerScript.stateScript.Hp = Mathf.Min(playerScript.stateScript.HpMax, playerScript.stateScript.Hp + heal);

        playerScript.canMove = true;

        GameObject player = AttackHinterscript.NewPlayer(HealSuccessSound, HealSuccessSound.length, 0f, 0f);
        player.transform.parent = spawner.transform;
        player.transform.position = Vector3.zero;

        endTime = Time.time + healTimeBack;
        deltaFov = 0;
        while (Time.time < endTime)
        {
            cam.fieldOfView = (oriFov + deltaFov) + (cam.fieldOfView - oriFov - deltaFov) * Mathf.Pow(0.02f, Time.deltaTime / healTimeBack);
            yield return null;
        }

        cam.fieldOfView = oriFov;

        Destroy(gameObject);
    }
}
