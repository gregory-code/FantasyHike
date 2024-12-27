using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchingRune : item
{
    private void Awake()
    {
        onUseItem += UseMyEffect;
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        if (recivingCharacter.currentHealth <= recivingCharacter.maxHealth / 4)
        {
            recivingCharacter.TakeDamage(-(99));
        }
        else
        {
            recivingCharacter.TakeDamage(-(usingItem.baseValue + usingCharacter.baseMagic));
        }
    }

    private void OnDestroy()
    {
        onUseItem -= UseMyEffect;
    }
}
