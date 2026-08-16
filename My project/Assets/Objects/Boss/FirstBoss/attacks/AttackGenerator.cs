
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using UnityEngine;

public class AttackGenerator : MonoBehaviour
{
    protected static readonly WaitForSeconds _waitForSeconds1 = new(1);
    [HideInInspector] public GameObject spawner;
    [HideInInspector] public GameObject target;
    public float difficulty = 0.2f;
    public float maxEnergyCost = 0;
    private Coroutine startAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        startAttack = StartCoroutine(StartAttack());
    }

    protected virtual void Update()
    {

    }

    void OnDestroy()
    {
        StopCoroutine(startAttack);
    }

    protected virtual IEnumerator StartAttack()
    {
        Debug.Log("called parent attack generator");
        yield return _waitForSeconds1;
        Destroy(gameObject);
    }
    public GameObject InstantiateGenerator(UnityEngine.Object perfab)
    {
        GameObject gen = (GameObject)Instantiate(perfab);
        AttackGenerator attGenScr = gen.GetComponent<AttackGenerator>();
        attGenScr.spawner = spawner;
        attGenScr.target = target;
        attGenScr.difficulty = difficulty;
        return gen;
    }

    public T InstantiateGenerator<T>(GameObject spawner) where T : AttackGenerator
    {
        T gen = spawner.AddComponent<T>();
        gen.spawner = spawner;
        gen.target = target;
        gen.difficulty = difficulty;
        return gen;
    }
    public T InstantiateAttack<T>(GameObject obj) where T : AttackScript
    {
        T att = obj.AddComponent<T>();
        att.spawner = spawner;
        att.target = target;
        att.AttackGenerator = this;
        return att;
    }
    public GameObject InstantiateAttack(UnityEngine.Object perfab)
    {
        GameObject attObj = (GameObject)Instantiate(perfab);
        if (attObj.TryGetComponent<AttackScript>(out AttackScript attScr))
        {
            attScr.spawner = spawner;
            attScr.target = target;
            attScr.AttackGenerator = this;
        }
        else
        {
            Debug.LogError("the perfab '" + perfab + "' does not contain AttackScript!");
        }
        return attObj;
    }

    protected AnimatorStateInfo animatorStateInfo;
    protected IEnumerator PlayAnimationAndLoop(AnimationClip clip, Action loopFunction = null)
    {
        Animator animator = spawner.GetComponent<Animator>();
        if (animator == null)
        {
            throw new System.Exception("PlayAnimationAndLoop require the spawner gameObject to have Animator component");
        }
        animator.Play(clip.name);
        animator.Update(0);
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (animatorStateInfo.normalizedTime < 1.0f)
        {
            if (loopFunction != null)
                loopFunction?.Invoke();
            yield return null;
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
