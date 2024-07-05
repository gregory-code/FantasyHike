using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class Enemy : character, IPointerClickHandler
{
    [SerializeField] enemyUI enemyUIPrefab;
    private enemyUI myUI;

    private BattleManager battleManager;

    [SerializeField] Animator attackIndicatorAnim;
    [SerializeField] Animator spellIndicatorAnim;

    [SerializeField] item itemPrefab;

    [SerializeField] int maxHealth;

    [SerializeField] float yUIadjustment;

    public bool hasActed;

    public delegate void OnEnemyClicked(Enemy enemy);
    public event OnEnemyClicked onEnemyClicked;

    public void Init(BattleManager battleManager)
    {
        this.battleManager = battleManager;

        myUI = Instantiate(enemyUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, yUIadjustment);
        myUI.Init(this, maxHealth);

        myUI.onDeath += Dead;
    }

    public void Attack(character player)
    {
        StartCoroutine(BasicAttackAnimation(player));
    }

    private void Dead()
    {
        PlayAnim("dead");
        FindObjectOfType<BattleManager>().RemoveEnemy(this);

        StartCoroutine(DeathSmoke());
    }

    public void SetAttackIndicator(bool state)
    {
        attackIndicatorAnim.SetBool("swing", state); 
        attackIndicatorAnim.SetBool("finish", !state);
    }

    public void SetSpellIndicator(bool state)
    {
        spellIndicatorAnim.SetBool("show", state);
        spellIndicatorAnim.SetBool("finish", !state);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onEnemyClicked?.Invoke(this);
    }
}
