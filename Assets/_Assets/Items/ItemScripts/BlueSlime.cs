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
        if(recivingCharacter.isPlayer)
        {
            recivingCharacter.Heal(baseValue);
        }
        else
        {
            recivingCharacter.TakeDamage(-baseValue);
        }

        onUseItem -= UseMyEffect;
        RemoveItem();
        Destroy(transform.gameObject);
    }
}
