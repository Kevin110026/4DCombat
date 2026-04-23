using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingSwordsattackgeneratorscript : attackgenerator
{
    public Object SwordPrefab;
    public AudioClip WhooshSound;
    private AudioSource audioSource;
    [HideInInspector] public int remainingSwords = 0;
    protected IEnumerator KeepDistance()
    {
        float desiredDist = Mathf.Lerp(difficulity, 12, 20);
        CharacterMovementScript cms = spawner.GetComponent<CharacterMovementScript>();
        while (true)
        {
            Vector3 diff = spawner.transform.position - target.transform.position;
            float dist = diff.magnitude;
            cms.MoveInVelocityXZ(diff.normalized, desiredDist - dist);
            yield return null;
        }
    }
    protected override IEnumerator StartAttack()
    {
        StartCoroutine(KeepDistance());
        yield return new WaitForSeconds(0.6f);
        audioSource = spawner.GetComponent<AudioSource>();
        int swordAmount = 3 + (difficulity >= 0.7 ? 1 : 0);
        List<GameObject> swords = new List<GameObject>(swordAmount);
        for (int i = 0; i < swordAmount; i++)
        {
            Vector3 shootDirection = i % 2 == 0 ? spawner.transform.right : -spawner.transform.right;
            // Debug.Log("hi");
            swords.Add((GameObject)Instantiate(SwordPrefab, spawner.transform.position, Quaternion.LookRotation(Quaternion.Euler(0, UnityEngine.Random.Range(-35f, 35f), 0) * shootDirection)));
            // Debug.Log("hi there");
            FlyingSwordsattackswordscript fsss = swords[i].GetComponent<FlyingSwordsattackswordscript>();
            fsss.spawner = spawner;
            fsss.target = target;
            fsss.attackgenerator = this;
            fsss.speed = fsss.GetMaxSpeed(difficulity);
            fsss.speedUp = true;
            fsss.parriedSound = Resources.Load<AudioClip>("parry-1-limbus");
            remainingSwords++;
            audioSource.PlayOneShot(WhooshSound);
            if (i >= 2)
            {
                fsss.visible = false;
                fsss.parriedSound = Resources.Load<AudioClip>("parry-3-limbus");
                if (i == 3)
                    fsss.GetComponent<AudioSource>().pitch = 1.2f;
            }
            yield return new WaitForSeconds(Mathf.Lerp(difficulity, 1f, 0.5f));
        }
        while (remainingSwords > 0)
            yield return null;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
