using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeWeaver : item
{
    [SerializeField] StatusEffect statusEffect;

    private void Awake()
    {
        onUseItem += UseMyEffect;
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        recivingCharacter.ApplyStatusEffect(usingCharacter, recivingCharacter, statusEffect, 3);
    }

    private void OnDestroy()
    {
        onUseItem -= UseMyEffect;
    }
}
