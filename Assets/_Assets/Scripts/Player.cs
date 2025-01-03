using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : character
{
    private SaveManager saveManager;

    [Header("Player")]
    [SerializeField] GameObject manaPopup;


    [SerializeField] playerUI playerUIPrefab;
    private playerUI myUI;

    [SerializeField] actionButtons actionButtonsPrefab;
    private actionButtons actionButtons;

    [SerializeField] Inventory inventory;

    private void Start()
    {
        saveManager = GameObject.FindObjectOfType<SaveManager>();

        myUI = Instantiate(playerUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, 1.3f);
        myUI.Init(this, GetSaveData().maxHealth, GetSaveData().maxMana, GetSaveData().currentHealth, GetSaveData().currentMana);

        myUI.onDeath += Dead;
        onStatusEffectApplied += ApplyStatus;
    }

    private void ApplyStatus(character usingCharacter, character recivingCharacter, StatusEffect applyEffect, int startingValue)
    {
        StatusEffect effect = Instantiate(applyEffect, myUI.statusEffectTransform);
        effect.Init(recivingCharacter, usingCharacter, startingValue);
    }

    public void AddStats(int moreMaxHealth, int moreMaxMana, int moreStrength, int moreMagic, int moreDefense)
    {
        myUI.AddedMaxHealth(moreMaxHealth);
        myUI.AddedMaxMana(moreMaxMana);

        baseStrength += moreStrength;
        //there's no save data for strength/magic/defense

        baseMagic += moreMagic;
        //there's no save data for strength/magic/defense

        baseDefense += moreDefense;
        //there's no save data for strength/magic/defense
    }

    public SaveData GetSaveData()
    {
        return saveManager.saveData;
    }

    private void Dead()
    {
        PlayAnim("dead");
        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1);
        FindObjectOfType<BattleManager>().StartOver();
    }

    public void Attack(character target, item preparedEffect)
    {
        StartCoroutine(BasicAttackAnimation(target));
    }

    public void SpellAttack(character target, item spell)
    {
        if (myUI.TryUseMana(spell.manaCost) == false)
            return;
        
        StartCoroutine(SpellAttackAnimation(target, spell));
    }

    public void UseConsumable(character target, item consumable)
    {
        StartCoroutine(ConsumableAnimation(target, consumable));
    }

    public int GetMana()
    {
        return myUI.GetMana();
    }

    public void AdjustMana(int cost)
    {
        myUI.TryUseMana(cost);
        if (cost <= -1)
        {
            SpawnPopup(manaPopup, -cost);
        }
    }

    public void ShowActions()
    {
        CharactersTurn(this);
        actionButtons = Instantiate(actionButtonsPrefab, transform);
        actionButtons.transform.localPosition = new Vector2(0, 0.9f);
        actionButtons.Init(this, inventory);
    }
}
