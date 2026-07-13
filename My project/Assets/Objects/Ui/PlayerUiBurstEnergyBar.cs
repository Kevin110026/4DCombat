using UnityEngine;
using UnityEngine.UI;

public class PlayerUiBurstEnergyBar : MonoBehaviour
{
    public CharacterScript characterScript;
    private Slider slider;
    void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = characterScript.stateScript.BurstEnergy / characterScript.stateScript.BurstEnergyMax;
    }
}
