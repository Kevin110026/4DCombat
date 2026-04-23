using UnityEngine;

public class Attackscript : MonoBehaviour
{
    protected bool canDamage = true;
    public float damage;
    public bool haveDamaged = false;
    public bool canBeParried = true;
    public float parriedPitch = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip parriedSound;
    [HideInInspector] public GameObject spawner;
    [HideInInspector] public GameObject target;
    [HideInInspector] public attackgenerator attackgenerator;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    virtual protected void DamagePlayer(GameObject player)
    {
        if (haveDamaged)
            return;
        Playerscript playerscript = player.GetComponent<Playerscript>();
        playerscript.Damaged(damage);
        haveDamaged = true;
        canBeParried = false;
    }
    public virtual void OnParried(GameObject parrier)
    {
        canDamage = false;
        canBeParried = false;
    }
}
