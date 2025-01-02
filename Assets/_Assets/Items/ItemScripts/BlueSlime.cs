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
            recivingCharacter.GetComponent<Player>().AdjustMana(-3);
        }
        else
        {
            recivingCharacter.TakeDamage(-baseValue - 2);
        }

        onUseItem -= UseMyEffect;
        RemoveItem();
        Destroy(transform.gameObject);
    }
}
