using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ForestHound : Enemy
{
    void Start()
    {
        onStartTurn += myTurn;
    }

    private void myTurn(character player)
    {
        StartCoroutine(BasicAttackAnimation(player));
        StartCoroutine(DoubleAttack(player));
    }

    private IEnumerator DoubleAttack(character player)
    {
        yield return new WaitForSeconds(1.5f);
        player.TakeDamage(-(baseStrength));
    }
}
