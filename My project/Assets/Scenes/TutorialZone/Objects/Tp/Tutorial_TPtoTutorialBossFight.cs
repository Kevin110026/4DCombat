using UnityEngine;

public class Tutorial_TPtoTutorialBossFight : MonoBehaviour
{
    [SerializeField] GameObject TutorialBoss;
    [SerializeField] GameObject TrainDummy;
    [SerializeField] Vector3 DesPos;
    [SerializeField] Vector3 DesRot;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerScript>(out PlayerScript ps))
        {
            ps.GetComponent<CharacterController>().enabled = false;
            ps.transform.SetPositionAndRotation(DesPos, Quaternion.Euler(DesRot));
            ps.GetComponent<CharacterController>().enabled = true;
            ps.stateScript.Hp = ps.stateScript.HpMax;
            ps.stateScript.BurstEnergy = ps.stateScript.BurstEnergyMax;
            TrainDummy.GetComponent<TrainDummyScript>().active = false;
            TutorialBoss.GetComponent<TutorialBossScript>().start = true;
            // TutorialBoss.GetComponent<TutorialBossScript>().autoAttack = true;
        }
    }
}
