using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicScroll : item
{
    private void Awake()
    {
        onUseItem += UseMyEffect;
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        recivingCharacter.TakeDamage(-(usingItem.baseValue + usingCharacter.baseMagic));
    }

    private void OnDestroy()
    {
        onUseItem -= UseMyEffect;
    }
}
