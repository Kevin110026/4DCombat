using System;
using System.Collections;
using UnityEngine;

public class TutorialBossSingleHitLRChargedImproved : TutorialBossSingleHitLRImproved
{
    override protected bool ShouldMove()
    {
        return GetComponent<TutorialBossCommonSwordAttackChargedgenerator>().isSlashing;
    }
}
