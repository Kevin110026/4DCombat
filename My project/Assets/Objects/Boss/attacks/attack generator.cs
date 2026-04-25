using System.Collections;
using UnityEngine;

public class attackgenerator : MonoBehaviour
{
    protected static readonly WaitForSeconds _waitForSeconds1 = new(1);
    [HideInInspector] public GameObject spawner;
    [HideInInspector] public GameObject target;
    public float difficulty = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        StartCoroutine(StartAttack());
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual IEnumerator StartAttack()
    {
        Debug.Log("called parent attack generator");
        yield return _waitForSeconds1;
        Object.Destroy(gameObject);
    }
}
