using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Enemy : character, IPointerClickHandler
{
    [Header("Enemy")]
    [SerializeField] Animator attackIndicatorAnim;
    [SerializeField] enemyUI enemyUIPrefab;
    private enemyUI myUI;

    [Header("Power Level")]
    public int power;
    [SerializeField] int maxHealthLevelUp;
    [SerializeField] int strengthLevelUp;
    [SerializeField] int magicLevelUp;
    [SerializeField] int defenseLevelUp;

    private GameObject enemyPos;

    private BattleManager battleManager;


    [SerializeField] int moneyYield;
    [SerializeField] item[] itemDropPool;

    [SerializeField] float yUIadjustment;

    private bool bHasActed;

    public void Init(BattleManager battleManager, int upgradeLevel, GameObject pos)
    {
        this.battleManager = battleManager;

        enemyPos = pos;

        maxHealth += upgradeLevel * maxHealthLevelUp;
        baseStrength += upgradeLevel * strengthLevelUp;
        baseMagic += upgradeLevel * magicLevelUp;
        baseDefense += upgradeLevel * defenseLevelUp;

        myUI = Instantiate(enemyUIPrefab, transform);
        myUI.transform.localPosition = new Vector2(0, yUIadjustment);
        myUI.Init(this, maxHealth);

        myUI.onDeath += Dead;
        onStatusEffectApplied += ApplyStatus;
    }

    private void ApplyStatus(character usingCharacter, character recivingCharacter, StatusEffect applyEffect, int startingValue)
    {
        StatusEffect effect = Instantiate(applyEffect, myUI.statusEffectTransform);
        effect.Init(recivingCharacter, usingCharacter, startingValue);
    }

    public GameObject GetPos()
    {
        return enemyPos;
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

        LootPool lootPool = FindObjectOfType<LootPool>();
        SaveManager saveManager = FindObjectOfType<SaveManager>();

        int randomItem = Random.Range(0, itemDropPool.Length);
        bool noItem = false;

        if(Random.Range(0, 4) >= 1)
        {

            for (int i = 0; i < saveManager.saveData.itemIDs.Count; i++)
            {
                if (itemDropPool[randomItem].itemName == saveManager.GetIDName(i))
                {
                    noItem = true;
                }
            }

            foreach(item item in lootPool.GetLootItems())
            {
                if (itemDropPool[randomItem].itemName == item.itemName)
                {
                    noItem = true;
                }
            }
        }
        else
        {
            noItem = true;
        }

        if (noItem)
        {
            Inventory playerInventory = FindObjectOfType<Inventory>();
            playerInventory.SetMoney(playerInventory.GetMoney() + moneyYield);
        }
        else
        {
            Transform spawn = lootPool.GetRandomSpawn();
            item droppedItem = Instantiate(itemDropPool[randomItem], spawn);
            droppedItem.Init(new Vector3((droppedItem.itemSize.x - 1) * -45, (droppedItem.itemSize.y - 1) * 45, 0), 0);
            lootPool.RegisterItemToLootPool(droppedItem);
        }
    }

    public void SetAttackIndicator(bool state)
    {
        attackIndicatorAnim.SetBool("swing", state); 
        attackIndicatorAnim.SetBool("finish", !state);
    }
}
