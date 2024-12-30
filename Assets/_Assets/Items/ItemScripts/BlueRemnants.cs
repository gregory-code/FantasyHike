using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueRemnants : item
{
    bool affectApplied = false;

    private void Awake()
    {
        onItemAdjusted += Equipped;
        StartCoroutine(SetUpDelegate());
    }

    private IEnumerator SetUpDelegate()
    {
        yield return new WaitForSeconds(0.2f);
        FindObjectOfType<Player>().onHealthChanged += HealthChanged;
    }

    private void HealthChanged(int change)
    {
        if(affectApplied && change <= -1)
        {
            FindObjectOfType<Player>().AdjustMana(-baseValue);
        }
    }

    private void Equipped(item item, bool added)
    {
        if (item != this)
            return;

        if (affectApplied == added)
            return;

        affectApplied = added;
    }

    private void OnDestroy()
    {
        onItemAdjusted -= Equipped;
        Player currentPlayer = FindObjectOfType<Player>();
        if (currentPlayer == null)
            return;

        currentPlayer.onHealthChanged += HealthChanged;
    }
}
