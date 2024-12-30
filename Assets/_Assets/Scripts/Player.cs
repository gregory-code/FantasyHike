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
        actionButtons = Instantiate(actionButtonsPrefab, transform);
        actionButtons.transform.localPosition = new Vector2(0, 0.9f);
        actionButtons.Init(this, inventory);
    }
}
