using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BlueSlime : Enemy
{
    void Start()
    {
        onStartTurn += myTurn;
    }

    private void myTurn(character player)
    {
        StartCoroutine(BasicAttackAnimation(player));
    }
}
