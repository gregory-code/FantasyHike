using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueRune : item
{
    private void Awake()
    {
        onUseItem += UseMyEffect;
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        Enemy[] allEnemies = FindObjectOfType<BattleManager>().GetAllEnemies();
        for(int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].TakeDamage(-(usingItem.baseValue + usingCharacter.baseMagic));
        }
    }

    private void OnDestroy()
    {
        onUseItem -= UseMyEffect;
    }
}
