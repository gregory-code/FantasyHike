using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : item
{
    bool affectApplied = false;

    private void Awake()
    {
        onItemAdjusted += Equipped;
    }

    private void Equipped(item item, bool added)
    {
        if (item != this)
            return;

        if (affectApplied == added)
            return;

        affectApplied = added;

        Player myPlayer = FindObjectOfType<Player>();
        myPlayer.baseStrength += (added) ? baseValue : -baseValue;
    }

    private void OnDestroy()
    {
        onItemAdjusted -= Equipped;
    }
}
