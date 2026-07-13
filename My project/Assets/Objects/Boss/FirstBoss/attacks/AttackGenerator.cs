
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackGenerator : MonoBehaviour
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

    protected virtual void Update()
    {

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
        T gen = obj.AddComponent<T>();
        gen.spawner = spawner;
        gen.target = target;
        return gen;
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
