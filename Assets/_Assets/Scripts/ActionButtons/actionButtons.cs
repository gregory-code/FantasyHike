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
    [SerializeField] consumable consumablePrefab;
    [SerializeField] Transform spellList;
    [SerializeField] spell spellPrefab;

    [SerializeField] Vector3 hiddenCanvas;
    [SerializeField] Vector3 shownCanvas;

    [SerializeField] Sprite unusableSprite;
    [SerializeField] Sprite spellSprite;
    [SerializeField] Sprite itemSprite;

    [SerializeField] Canvas[] buttonCanvases;

    public bool hovingSpecialCanvas;

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

        foreach (item itemSpell in itemSpells)
        {
            spell newSpell = Instantiate(spellPrefab, spellList);
            newSpell.Init(this, itemSpell.myEffect.manaCost, itemSpell.myEffect.spellName, itemSpell.myEffect.spellIcon);
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
            if(hovingSpecialCanvas)
            {
                hovingSpecialCanvas = false;
                return;
            }
            StartCoroutine(TouchedScreen());
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

    private void MadeChoice(Enemy enemy)
    {
        TargettingAttack(false);
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
                enemy.onEnemyClicked += owner.Attack;
            }
            else
            {
                enemy.onEnemyClicked -= MadeChoice;
                enemy.onEnemyClicked -= owner.Attack;
            }
        }
    }

}
