
using System;
using System.Collections;
using UnityEngine;

public class AttackGenerator : MonoBehaviour
{
    protected static readonly WaitForSeconds _waitForSeconds1 = new(1);
    [HideInInspector] public GameObject spawner;
    [HideInInspector] public GameObject target;
    [HideInInspector] public float parriedPitch = 1.0f;
    public float difficulty = 0.2f;
    public float maxEnergyCost = 0;
    private Coroutine startAttack;

    protected CameraScript cameraScript;
    virtual protected void Awake()
    {
        cameraScript = Camera.allCameras[0].GetComponentInParent<CameraScript>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        startAttack = StartCoroutine(StartAttack());
    }

    protected virtual void Update()
    {

    }

    virtual protected void OnDestroy()
    {
        StopCoroutine(startAttack);
    }

    protected virtual IEnumerator StartAttack()
    {
        Debug.Log("called parent attack generator");
        yield return _waitForSeconds1;
        Destroy(gameObject);
    }

    public virtual void OnParried(GameObject parrier)
    {
        // Debug.Log("parent OnParried called");
    }
    public GameObject InstantiateGenerator(UnityEngine.Object perfab)
    {
        GameObject gen = (GameObject)Instantiate(perfab);
        AttackGenerator[] attack_generator_scripts = gen.GetComponents<AttackGenerator>();
        foreach (AttackGenerator attGenScr in attack_generator_scripts)
        {
            attGenScr.spawner = spawner;
            attGenScr.target = target;
            attGenScr.difficulty = difficulty;
        }
        return gen;
    }

    public T InstantiateGenerator<T>(GameObject spawner) where T : AttackGenerator
    {
        T gen = spawner.AddComponent<T>();
        gen.spawner = spawner;
        gen.target = target;
        gen.difficulty = difficulty;
        gen.parriedPitch = parriedPitch;
        return gen;
    }
    public T InstantiateAttack<T>(GameObject obj) where T : AttackScript
    {
        // GameObject attObj = new GameObject();
        // T att = attObj.AddComponent<T>();
        // attObj.transform.SetParent(obj.transform);

        T att = obj.AddComponent<T>();
        att.spawner = spawner;
        att.target = target;
        att.parriedPitch = parriedPitch;
        att.attackGenerator = this;
        att.dontDestroyGameobject = true;
        return att;
    }
    public GameObject InstantiateAttack(UnityEngine.Object perfab)
    {
        GameObject attObj = (GameObject)Instantiate(perfab);
        if (attObj.TryGetComponent<AttackScript>(out AttackScript attScr))
        {
            attScr.spawner = spawner;
            attScr.target = target;
            attScr.parriedPitch = parriedPitch;
            attScr.attackGenerator = this;
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
            // Debug.Log("looping " + clip.name + " " + Time.time);
        }
    }

    protected void LoadHintSoundPath(GameObject hinter, GameObject parent, HintSoundPath path, float duration)
    {
        StartCoroutine(LoadHintSoundPathIEnumerator(hinter, parent, path, duration));
    }
    IEnumerator LoadHintSoundPathIEnumerator(GameObject hinter, GameObject parent, HintSoundPath path, float duration)
    {
        hinter.transform.parent = parent.transform;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            hinter.transform.localPosition = path.Pos((Time.time - startTime) / duration);
            yield return null;
        }
        hinter.transform.localPosition = path.Pos(1);
    }

    protected IEnumerator LoopForSeconds(float seconds, Action loopFunction = null)
    {
        float start = Time.time;
        while (Time.time < start + seconds)
        {
            if (loopFunction != null)
                loopFunction?.Invoke();
            yield return null;
        }
    }


    //  for parry's delayed hit
    //  dont put multiple attack script that will destroy its gameobject in one gameobject
    protected virtual void ScheduleDestroy(GameObject obj)
    {
        if (obj == null)
            return;
        MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component is AttackScript attscr)
            {
                attscr.ScheduleDestroy();
            }
            else
            {
                component.enabled = false;
            }
        }
        // Destroy(obj);
    }
    protected virtual void ScheduleDestroy(AttackScript attscr)
    {
        if (attscr == null)
            return;
        attscr.ScheduleDestroy();
    }
}
