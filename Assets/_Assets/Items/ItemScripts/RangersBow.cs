using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangersBow : item
{
    private void Awake()
    {
        onUseItem += UseMyEffect;
        onUseOffScreenItem += ActivateEarlyEffect;
    }

    private void ActivateEarlyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        StartCoroutine(SpawnArrows(usingItem, usingCharacter, recivingCharacter));
    }

    private IEnumerator SpawnArrows(item usingItem, character usingCharacter, character recivingCharacter)
    {
        Enemy[] allEnemies = FindObjectOfType<BattleManager>().GetAllEnemies();
        for (int i = 0; i < allEnemies.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);
            projectile spellProjectile = Instantiate(usingItem.spellProjectile, new Vector3(-300, 200, 4), usingItem.spellProjectile.transform.rotation);
            spellProjectile.Init(allEnemies[i], usingItem, usingCharacter, true);
        }
    }

    private void UseMyEffect(item usingItem, character usingCharacter, character recivingCharacter)
    {
        recivingCharacter.TakeDamage(-(usingItem.baseValue + usingCharacter.baseStrength));
    }

    private void OnDestroy()
    {
        onUseItem -= UseMyEffect;
        onUseOffScreenItem -= ActivateEarlyEffect;
    }
}
