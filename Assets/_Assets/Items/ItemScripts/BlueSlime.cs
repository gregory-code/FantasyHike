using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSlime : item
{
    private void Awake()
    {
        onUseItem += UseMyEffect;
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        if (recivingCharacter.maxHealth / 3 > usingItem.baseValue)
        {
            recivingCharacter.Heal(recivingCharacter.maxHealth / 3);
        }
        else
        {
            recivingCharacter.Heal(usingItem.baseValue);
        }

        onUseItem -= UseMyEffect;
        RemoveItem();
        Destroy(transform.gameObject);
    }
}
