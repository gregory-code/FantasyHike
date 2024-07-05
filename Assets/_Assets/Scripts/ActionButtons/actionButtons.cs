using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionButtons : MonoBehaviour
{
    [SerializeField] Animator actionAnim;
    [SerializeField] actionBtn atkBtn;
    [SerializeField] actionBtn spellBtn;
    [SerializeField] actionBtn itemBtn;

    [SerializeField] Canvas spellCanvas;
    [SerializeField] Canvas itemCanvas;

    private Player owner;
    private Inventory inventory;

    [SerializeField] Transform consumableList;
    [SerializeField] Transform consumbleDescription;

    [SerializeField] Transform spellList;
    [SerializeField] Transform spellDescription;

    [SerializeField] usableItem usableSpellPrefab;
    [SerializeField] usableItem usableConsumablePrefab;

    [SerializeField] Vector3 hiddenCanvas;
    [SerializeField] Vector3 shownCanvas;

    [SerializeField] Sprite unusableSprite;
    [SerializeField] Sprite spellSprite;
    [SerializeField] Sprite itemSprite;

    [SerializeField] Canvas[] buttonCanvases;

    public bool hovingSpecialCanvas;

    private bool inspectingItem;
    private usableItem currentUsableItem;

    private bool targettingAttack;
    private bool bInSelection;

    public void Init(Player player, Inventory inventory)
    {
        owner = player;
        this.inventory = inventory;

        atkBtn.onActionClicked += actionClicked;
        spellBtn.onActionClicked += actionClicked;
        itemBtn.onActionClicked += actionClicked;

        actionAnim.SetBool("choosing", true);

        spellCanvas.worldCamera = Camera.main;
        itemCanvas.worldCamera = Camera.main;

        List<item> itemSpells = inventory.GetItemType(itemEffect.itemType.spell);
        List<item> itemConsumables = inventory.GetItemType(itemEffect.itemType.consumable);

        spellBtn.InteractState(itemSpells.Count > 0, unusableSprite, spellSprite);
        itemBtn.InteractState(itemConsumables.Count > 0, unusableSprite, itemSprite);

        spellList.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f, itemSpells.Count);

        foreach (item itemSpell in itemSpells)
        {
            usableItem newItem = Instantiate(usableSpellPrefab, spellList);
            newItem.Init(itemSpell, this, spellList, spellDescription, itemSpell.myEffect.spellName, itemSpell.myEffect.spellIcon, itemSpell.myEffect.manaCost, owner.GetMana());
            newItem.onSelectItem += SelectItem;
        }
        SortSpells();

        foreach (item itemConsumable in itemConsumables)
        {
            usableItem newItem = Instantiate(usableConsumablePrefab, consumableList);
            newItem.Init(itemConsumable, this, consumableList, consumbleDescription, itemConsumable.myEffect.itemName, itemConsumable.myEffect.itemIcon, 0, 0);
            newItem.onSelectItem += SelectItem;
        }

        foreach (Canvas canvas in buttonCanvases)
        {
            canvas.worldCamera = Camera.main;

        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && bInSelection)
        {
            if(inspectingItem)
            {
                currentUsableItem.transform.SetParent(spellList);
                currentUsableItem.list.gameObject.SetActive(true);
                inspectingItem = false;

                if(currentUsableItem.IsAttackingSpell())
                {
                    TargettingSpell(false, currentUsableItem);
                }

                if(currentUsableItem.description.transform.childCount >= 1)
                {
                    Destroy(currentUsableItem.description.transform.GetChild(0).gameObject);
                }

                if(currentUsableItem.isSpell)
                {
                    SortSpells();
                }


                StopAllCoroutines();
                return;
            }

            if(hovingSpecialCanvas)
            {
                hovingSpecialCanvas = false;
                return;
            }
            StartCoroutine(TouchedScreen());
        }
    }

    private void SelectItem(usableItem selectingItem)
    {
        if (inspectingItem)
            return;

        inspectingItem = true;
        currentUsableItem = selectingItem;

        if(selectingItem.IsAttackingSpell())
        {
            TargettingSpell(true, selectingItem);
        }

        description descrip = Instantiate(selectingItem.itemOwner.myEffect.itemDescription, selectingItem.description);
        descrip.SetStats(owner.baseStrength, owner.baseMagic);
        descrip.transform.localPosition = Vector3.zero;

        selectingItem.transform.SetParent(selectingItem.list.transform.parent);
        StartCoroutine(MoveUsableItem(selectingItem.transform));

        selectingItem.list.gameObject.SetActive(false);
    }

    private IEnumerator MoveUsableItem(Transform transformToMove)
    {
        Vector3 destination = new Vector3(0.05f, 1f, 0);
        while(Vector3.Distance(transformToMove.localPosition, destination) > 0.01f)
        {
            transformToMove.localPosition = Vector3.Lerp(transformToMove.localPosition, destination, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MoveSpells(Vector3 destination)
    {
        while(Vector3.Distance(spellCanvas.transform.position, destination) >= 0.2f)
        {
            spellCanvas.transform.position = Vector3.Lerp(spellCanvas.transform.position, destination, 8 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator MoveItems(Vector3 destination)
    {
        while (Vector3.Distance(itemCanvas.transform.position, destination) >= 0.2f)
        {
            itemCanvas.transform.position = Vector3.Lerp(itemCanvas.transform.position, destination, 8 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void MadeChoice(Enemy enemy, itemEffect preparedEffect)
    {
        TargettingAttack(false);

        if(currentUsableItem != null)
        {
            if(currentUsableItem.IsAttackingSpell())
            {
                TargettingSpell(false, currentUsableItem);
            }
        }

        Destroy(this.gameObject);
        actionAnim.SetBool("choosing", false);
    }

    private IEnumerator TouchedScreen()
    {
        yield return new WaitForEndOfFrame();

        actionAnim.SetTrigger("Reset");
        bInSelection = false;

        if (targettingAttack)
        {
            TargettingAttack(false);
        }

        SetSpellCanvas(hiddenCanvas);
        SetItemCanvas(hiddenCanvas);
    }

    private void actionClicked(string ID)
    {
        if (bInSelection)
            return;

        StartCoroutine(actionClickedDelay(ID));
    }

    private IEnumerator actionClickedDelay(string ID)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        bInSelection = true;

        actionAnim.SetTrigger(ID);

        switch (ID)
        {
            case "attack":
                TargettingAttack(true);
                break;

            case "spell":
                SetSpellCanvas(shownCanvas);
                break;

            case "item":
                SetItemCanvas(shownCanvas);
                break;
        }
    }

    private void SetSpellCanvas(Vector3 destination)
    {
        StopCoroutine(MoveSpells(destination));

        StartCoroutine(MoveSpells(destination));
    }

    private void SetItemCanvas(Vector3 destination)
    {
        StopCoroutine(MoveItems(destination));

        StartCoroutine(MoveItems(destination));
    }

    private void SortSpells()
    {
        usableItem[] spells = spellList.GetComponentsInChildren<usableItem>();

        for (int i = 0; i <= 10; i++)
        {
            foreach (usableItem spell in spells)
            {
                if (spell.itemOwner.myEffect.manaCost == i)
                {
                    spell.transform.SetAsLastSibling();
                }
            }
        }
    }

    private void TargettingAttack(bool state)
    {
        targettingAttack = state;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        
        foreach (Enemy enemy in enemies)
        {
            enemy.SetAttackIndicator(state);

            if (state)
            {
                enemy.onEnemyClicked += MadeChoice;
                enemy.preparedEffect = null;
                enemy.onEnemyClicked += owner.Attack;
            }
            else
            {
                enemy.onEnemyClicked -= MadeChoice;
                enemy.onEnemyClicked -= owner.Attack;
            }
        }
    }

    private void TargettingSpell(bool state, usableItem usingSpell)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.SetSpellIndicator(state);

            if (state)
            {
                enemy.onEnemyClicked += MadeChoice;
                enemy.preparedEffect = usingSpell.itemOwner.myEffect;
                enemy.onEnemyClicked += owner.SpellAttack;
            }
            else
            {
                enemy.onEnemyClicked -= MadeChoice;
                enemy.preparedEffect = null;
                enemy.onEnemyClicked -= owner.SpellAttack;
            }
        }
    }

    /*private void TargettingConsumable(bool state, usableItem usingSpell)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.SetSpellIndicator(state);

            if (state)
            {
                enemy.onEnemyClicked += MadeChoice;
                enemy.preparedEffect = usingSpell.itemOwner.myEffect;
                enemy.onEnemyClicked += owner.SpellAttack;
            }
            else
            {
                enemy.onEnemyClicked -= MadeChoice;
                enemy.preparedEffect = null;
                enemy.onEnemyClicked -= owner.SpellAttack;
            }
        }
    }*/

}
