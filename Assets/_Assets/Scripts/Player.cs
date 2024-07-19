using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Player : character
{
    [SerializeField] playerUI playerUIPrefab;
    private playerUI myUI;

    [SerializeField] actionButtons actionButtonsPrefab;
    private actionButtons actionButtons;

    [SerializeField] Inventory inventory;

    [SerializeField] int maxHealth;
    [SerializeField] int maxMana;

    private void Start()
    {
        myUI = Instantiate(playerUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, 1.3f);
        myUI.Init(this, maxHealth, maxMana);
    }

    public void Attack(character target, itemEffect preparedEffect)
    {
        StartCoroutine(BasicAttackAnimation(target));
    }

    public void SpellAttack(character target, itemEffect spell)
    {
        if (myUI.TryUseMana(spell.manaCost) == false)
            return;
        
        StartCoroutine(SpellAttackAnimation(target, spell));
    }

    public void UseConsumable(character target, itemEffect consumable)
    {
        StartCoroutine(ConsumableAnimation(target, consumable));
    }

    private IEnumerator ConsumableAnimation(character target, itemEffect consumeable)
    {
        PlayAnim("elixer");
        yield return new WaitForSeconds(animLength["elixer"] / 3f);
        yield return new WaitForSeconds(animLength["elixer"] / 3f);

        target.Heal(consumeable.baseValue);

        PlayAnim("idle");

        yield return new WaitForSeconds(0.3f);
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
