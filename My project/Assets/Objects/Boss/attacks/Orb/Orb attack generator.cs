using UnityEngine;
using UnityEngine.InputSystem;

public class Orbattackgenerator : MonoBehaviour
{
    public GameObject target;
    public GameObject attack;

    public float range = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame) spawn();
    }
    void spawn()
    {
        Vector3 pos = target.transform.position;
        Vector2 offset = Random.insideUnitCircle * range;
        pos.x += offset.x;
        pos.z += offset.y;
        Instantiate(attack, pos, Quaternion.identity);
        // Instantiate(attack, gameObject.transform.position, Quaternion.identity);
    }
}
