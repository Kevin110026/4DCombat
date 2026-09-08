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

        GameObject hintPlayer = AttackHinterscript.NewPlayer(HealChargingSound, healTimeFront - 0.1f, 0.1f, 0f);
        hintPlayer.transform.parent = spawner.transform;
        hintPlayer.transform.localPosition = Vector3.zero;

        Camera cam = spawner.GetComponent<PlayerControlScript>().playerCamera;
        CameraScript cameraScript = Camera.allCameras[0].GetComponent<CameraScript>();
        float endTime = Time.time + healTimeFront;
        cameraScript.FovChangeLinear(-12, healTimeFront - 0.1f);
        while (Time.time < endTime)
        {
            float eDiff = (Time.time - endTime + healTimeFront) / healTimeFront * energyCost - energyConsumed;
            playerScript.ConsumeBurstEnergy(eDiff);
            energyConsumed += eDiff;
            yield return null;
        }
        playerScript.ConsumeBurstEnergy(energyCost - energyConsumed);
        playerScript.stateScript.Hp = Mathf.Min(playerScript.stateScript.HpMax, playerScript.stateScript.Hp + heal);

        playerScript.canMove = true;

        hintPlayer = AttackHinterscript.NewPlayer(HealSuccessSound, HealSuccessSound.length, 0f, 0f);
        hintPlayer.transform.parent = spawner.transform;
        hintPlayer.transform.localPosition = Vector3.zero;

        endTime = Time.time + healTimeBack;
        cameraScript.FovChangeLinear(12, healTimeBack);


        Destroy(gameObject);
    }
}
