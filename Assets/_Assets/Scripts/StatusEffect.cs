using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatusEffect : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI statusValueText;
    [SerializeField] Image statusIcon;

    [Header("Special Interactions")]
    [SerializeField] string statusName;
    [SerializeField] Sprite statusSprite;
    [SerializeField] GameObject procEffects;

    [SerializeField] int statusID;
    private int value;

    private character owner;
    private character inflicter;

    public void Init(character owner, character inflicter, int startingValue)
    {
        this.owner = owner;
        this.inflicter = inflicter;

        value = startingValue;

        statusValueText.text = value + "";
        statusIcon.sprite = statusSprite;

        owner.onStartTurn += OwnerTurnStart;
        owner.onEndTurn += OwnerTurnEnd;
        inflicter.onStartTurn += InflicterTurnStart;
        inflicter.onEndTurn += InflicterTurnEnd;

        owner.onHealthChanged += OwnerHealthChanged;
    }

    private void OwnerHealthChanged(int change)
    {
        
    }

    private void OwnerTurnStart(character character)
    {
        switch(statusID)
        {
            case 0: // Life Weaver
                Proc(true);
                owner.Heal(3);
                owner.baseStrength += 1;
                LoseValue(1);
                break;
        }
    }

    private void OwnerTurnEnd(character character)
    {
        switch (statusID)
        {
            case 0: // Life Weaver

                break;

            default:
                LoseValue(1);
                break;
        }
    }

    private void InflicterTurnStart(character character)
    {

    }

    private void InflicterTurnEnd(character character)
    {
        
    }

    public void StatusPressed()
    {

    }

    public int GetID()
    {
        return statusID;
    }

    private void AddValue(int valueAdded)
    {
        value += valueAdded;
    }

    private void LoseValue(int valueLost)
    {
        value -= valueLost;
        statusValueText.text = value + "";

        if (value <= 0)
        {
            statusValueText.text = 0 + "";
            RemoveThisStatusEffect();
        }
    }

    private void Proc(bool bProcOnMyself)
    {
        GameObject procEffect = Instantiate(procEffects, (bProcOnMyself) ? owner.transform.position : inflicter.transform.position, procEffects.transform.rotation);
        Destroy(procEffect, 0.6f);
    }

    private void RemoveThisStatusEffect()
    {
        owner.onStartTurn -= OwnerTurnStart;
        owner.onEndTurn -= OwnerTurnEnd;
        inflicter.onStartTurn -= InflicterTurnStart;
        inflicter.onEndTurn -= InflicterTurnEnd;

        owner.onHealthChanged -= OwnerHealthChanged;

        Destroy(this.gameObject);
    }
}
