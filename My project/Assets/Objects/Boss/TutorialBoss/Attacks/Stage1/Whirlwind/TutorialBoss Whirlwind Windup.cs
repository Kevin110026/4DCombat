using System.Collections;
using UnityEngine;

public class TutorialBossWhirlwindWindup : TutorialBossAnimatedAttack
{
    
    protected virtual void WindupLoopAction()
    {
        tutorialBossScript.FaceTarget();
    }
    protected override IEnumerator StartAttack()
    {
        AudioClip slash_sound = Resources.Load<AudioClip>("dragon-studio-whoosh-motion-405445");
        AudioSource blade_audioSource = blade.GetComponent<AudioSource>();
        blade_audioSource.clip = slash_sound;
        blade_audioSource.pitch = Random.Range(0.95f, 1.05f);
        blade_audioSource.time = 0.3f;
        blade_audioSource.volume = 0.6f;
        blade_audioSource.Play();
        
        yield return StartCoroutine(PlayAnimationAndLoop(WindupAnimation, WindupLoopAction));
        Destroy(gameObject);
    }
}
