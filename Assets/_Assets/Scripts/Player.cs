using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public void Attack(Enemy target)
    {
        StartCoroutine(BasicAttackAnimation(target));
    }

    public void ShowActions()
    {
        actionButtons = Instantiate(actionButtonsPrefab, transform);
        actionButtons.transform.localPosition = new Vector2(0, 0.9f);
        actionButtons.Init(this, inventory);
    }
}
