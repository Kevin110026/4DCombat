using System.Collections;
using Mono.Cecil;
using UnityEngine;

public class DrawingSlashConsecutiveattackgeneratorscript : AttackGenerator
{
    public Object SlashPerfab;
    protected override IEnumerator StartAttack()
    {
        int slashCount = 2 + (int)Mathf.Round(Mathf.Lerp(0f, 2f, difficulty)) + Random.Range(0, 3);
        for (int i = 0; i < slashCount; i++)
        {
            GameObject slashGen = (GameObject)Instantiate(SlashPerfab);
            DrawingSlashattackgeneratorscript ag = slashGen.GetComponent<DrawingSlashattackgeneratorscript>();
            ag.spawner = spawner;
            ag.target = target;
            ag.invisable = true;
            ag.randomAngle = true;
            ag.unparriedable = Mathf.Lerp(0f, 0.3f, difficulty) + 0f >= Random.Range(0f, 1f);
            ag.lowcd = true;
            if (i != slashCount - 1)
                ag.parriedSound = Resources.Load<AudioClip>("parry-1-limbus");
            else
                ag.parriedSound = Resources.Load<AudioClip>("parry-3-limbus");
            while (slashGen != null)
                yield return null;
        }
        Destroy(gameObject);
    }
}
