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

    [SerializeField] item itemPrefab;
    [SerializeField] itemEffect itemDrops;

    [SerializeField] int maxHealth;

    [SerializeField] float yUIadjustment;

    public bool hasActed;

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

        StartCoroutine(SpawnItem());

        StartCoroutine(DeathSmoke());
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(0.4f);

        //item droppedItem = Instantiate(itemPrefab, FindObjectOfType<Inventory>().transform);
        //droppedItem.transform.localPosition = transform.position;
        //droppedItem.myEffect = itemDrops;
    }

    public void SetAttackIndicator(bool state)
    {
        attackIndicatorAnim.SetBool("swing", state); 
        attackIndicatorAnim.SetBool("finish", !state);
    }
}
