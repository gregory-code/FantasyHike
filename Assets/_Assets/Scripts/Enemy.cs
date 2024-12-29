using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class Enemy : character, IPointerClickHandler
{
    [Header("Enemy")]
    [SerializeField] enemyUI enemyUIPrefab;
    private enemyUI myUI;

    private BattleManager battleManager;

    [SerializeField] Animator attackIndicatorAnim;

    [SerializeField] item[] itemDropPool;

    [SerializeField] float yUIadjustment;

    private bool bHasActed;

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
        battleManager.RemoveEnemy(this);

        StartCoroutine(SpawnItem());

        StartCoroutine(DeathSmoke());
    }

    public bool HasActed()
    {
        return bHasActed;
    }

    public void SetHasActed(bool state)
    {
        bHasActed = state;
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(0.4f);

        Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        item droppedItem = Instantiate(itemDropPool[0], canvas.transform);
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, camera, out Vector2 localPosition);
        droppedItem.transform.localPosition = localPosition;
        droppedItem.transform.SetParent(FindObjectOfType<Inventory>().transform);
    }

    public void SetAttackIndicator(bool state)
    {
        attackIndicatorAnim.SetBool("swing", state); 
        attackIndicatorAnim.SetBool("finish", !state);
    }
}
