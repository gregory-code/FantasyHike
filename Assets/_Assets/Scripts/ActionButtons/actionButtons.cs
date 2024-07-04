using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionButtons : MonoBehaviour
{
    [SerializeField] Animator actionAnim;
    [SerializeField] actionBtn atkBtn;
    [SerializeField] actionBtn spellBtn;
    [SerializeField] actionBtn itemBtn;

    private Player owner;
    private Inventory inventory;

    [SerializeField] Sprite unusableSprite;
    [SerializeField] Sprite spellSprite;
    [SerializeField] Sprite itemSprite;

    [SerializeField] Canvas[] buttonCanvases;

    private bool targettingAttack;
    private bool bInSelection;

    public void Init(Player player, Inventory inventory)
    {
        owner = player;

        atkBtn.onActionClicked += actionClicked;
        spellBtn.onActionClicked += actionClicked;
        itemBtn.onActionClicked += actionClicked;

        actionAnim.SetBool("choosing", true);

        spellBtn.InteractState(inventory.GetItemType(itemEffect.itemType.spell).Count > 0, unusableSprite, spellSprite);
        itemBtn.InteractState(inventory.GetItemType(itemEffect.itemType.spell).Count > 0, unusableSprite, itemSprite);

        foreach (Canvas canvas in buttonCanvases)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0) && bInSelection)
        {
            StartCoroutine(TouchedScreen());
        }
    }

    private void MadeChoice(Enemy enemy)
    {
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
                break;

            case "item":
                break;
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
