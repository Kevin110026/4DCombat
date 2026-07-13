using System;
using System.Collections;
using UnityEngine;

public abstract class TutorialBossAnimatedAttack : TutorialBossAttackGenerator
{
    
    public AnimationClip WindupAnimation;
    public float WindupAnimationPlaySpeed = -1;
    public AnimationClip AttackAnimation;
    public float AttackAnimationPlaySpeed = -1;
}
