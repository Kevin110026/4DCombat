using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDefeatedgenerator : PlayerAttackGenerator
{
    protected override IEnumerator StartAttack()
    {
        playerScript.canAct = false;
        playerScript.canMove = false;
        playerScript.canTurn = false;
        GameObject cam = spawner.GetComponent<PlayerControlScript>().playerCamera;
        CameraScript cameraScript = cam.GetComponent<CameraScript>();
        cameraScript.fieldOfView = 100;
        cameraScript.FovChangeLinear(-10, 3);
        cameraScript.ScreenShakeFovSHM(20, 10, 3, 10 * 2);
        cameraScript.ScreenShakeRotSHM(20, 10, 3, 10 * 2);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
