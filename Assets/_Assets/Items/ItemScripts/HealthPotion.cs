using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HealthPotion : item
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
