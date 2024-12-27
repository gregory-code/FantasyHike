using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : character
{
    [Header("Player")]
    [SerializeField] int maxMana;

    [SerializeField] playerUI playerUIPrefab;
    private playerUI myUI;

    [SerializeField] actionButtons actionButtonsPrefab;
    private actionButtons actionButtons;

    [SerializeField] Inventory inventory;

    private void Start()
    {
        myUI = Instantiate(playerUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, 1.3f);
        myUI.Init(this, maxHealth, maxMana);
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

    public void ShowActions()
    {
        actionButtons = Instantiate(actionButtonsPrefab, transform);
        actionButtons.transform.localPosition = new Vector2(0, 0.9f);
        actionButtons.Init(this, inventory);
    }
}
