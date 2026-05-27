using UnityEngine;
using UnityEngine.UI;

public class PlayerUiHp : MonoBehaviour
{
    public Playerscript playerscript;
    private GameObject valueBar;
    private Slider slider;
    void Awake()
    {
        valueBar = gameObject;
        slider = gameObject.GetComponent<Slider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = playerscript.hp / 100;
    }
}
