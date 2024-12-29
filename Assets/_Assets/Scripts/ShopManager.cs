using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Image sellItemImage;
    [SerializeField] TextMeshProUGUI sellItemPrice;
    [SerializeField] ShopItem shopItemTemplate;

    [SerializeField] ParticleSystem moneyPoofVFX;

    private SaveManager saveManager;
    private Inventory playerInventory;

    [SerializeField] SellMenu sellMenu;
    [SerializeField] Vector3 nonAvaiableSellMenu;
    [SerializeField] Vector3 avaiableSellMenu;
    [SerializeField] Vector3 shownSellMenu;

    private List<item> commonItems = new List<item>();
    private List<item> uncommonItems = new List<item>();
    private List<item> rareItems = new List<item>();
    private List<item> epicItems = new List<item>();
    private List<item> legendaryItems = new List<item>();

    bool sellAvailable;
    bool showSell;

    public void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        playerInventory = FindObjectOfType<Inventory>();

        foreach(item foundItem in playerInventory.GetItemLibrary())
        {
            switch(foundItem.myRariety)
            {
                case item.itemRariety.common:
                    commonItems.Add(foundItem);
                    break;

                case item.itemRariety.uncommon:
                    uncommonItems.Add(foundItem);
                    break;

                case item.itemRariety.rare:
                    rareItems.Add(foundItem);
                    break;

                case item.itemRariety.epic:
                    epicItems.Add(foundItem);
                    break;

                case item.itemRariety.legendary:
                    legendaryItems.Add(foundItem);
                    break;
            }
        }

        switch (saveManager.saveData.level)
        {
            case 0:
                GiveSelection(true, commonItems);
                AllowSelling(true);
                break;

            default:
                AllowSelling(false);
                break;
        }
    }

    private void GiveSelection(bool freeItems, List<item> itemGroup)
    {
        for(int i = 0; i < 3; i++)
        {
            bool uniqueOption = false;
            item newItem = null;
            do
            {
                int randomItem = Random.Range(0, itemGroup.Count);
                newItem = itemGroup[randomItem];
                for(int x = 0; x < saveManager.saveData.itemIDs.Count; x++)
                {
                    if (newItem.itemName != saveManager.GetIDName(x))
                    {
                        //uniqueOption = true;
                    }
                }
            }
            while (uniqueOption == true);

            Transform mainCanvasv = GameObject.FindWithTag("MainCanvas").transform;
            ShopItem newShopItem = Instantiate(shopItemTemplate, mainCanvasv);
            newShopItem.transform.localPosition = new Vector3((i * 300) - 300, 100, 0);
            newShopItem.Init(newItem, freeItems);
        }
    }

    private void AllowSelling(bool state)
    {
        sellAvailable = state;
        StopAllCoroutines();
        StartCoroutine(MoveSellMenu());
    }

    public void ItemDrag(item itemDragging, bool startDrag)
    {
        if (itemDragging == null)
            return;

        showSell = startDrag;

        int itemValue = Mathf.RoundToInt(itemDragging.buyPrice / 2);

        sellItemImage.sprite = itemDragging.itemIcon;
        sellItemPrice.text = "+" + itemValue;

        StopAllCoroutines();
        StartCoroutine(MoveSellMenu());

        if(startDrag == false && sellMenu.bHoveringSellMenu)
        {
            Inventory playerInventory = FindObjectOfType<Inventory>();
            //sell it here
            playerInventory.RegistarItem(itemDragging, false);
            itemDragging.RemoveItem();
            saveManager.saveData.itemIDs.Remove($"{itemDragging.itemName}/{itemDragging.GetGridPos().x}_{itemDragging.GetGridPos().y}");
            Destroy(itemDragging.gameObject);

            ParticleSystem moneyBurst = Instantiate(moneyPoofVFX, new Vector3(2.4f,1.3f,4), moneyPoofVFX.transform.rotation);
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[moneyBurst.emission.burstCount];
            bursts[0].count = itemValue;
            moneyBurst.emission.SetBurst(0, bursts[0]);
            moneyBurst.Play();

            playerInventory.SetMoney(playerInventory.GetMoney() + itemValue);
        }
    }

    private IEnumerator MoveSellMenu()
    {
        Vector3 pos = new Vector3(0, 0, 0);

        if(sellAvailable && showSell)
            pos = shownSellMenu;

        if(sellAvailable && !showSell)
            pos = avaiableSellMenu;

        if(sellAvailable == false)
            pos = nonAvaiableSellMenu;

        while (Vector3.Distance(sellMenu.transform.localPosition, pos) >= 0.1f)
        {
            yield return new WaitForEndOfFrame();
            Vector3 newMovePos = Vector3.Lerp(sellMenu.transform.localPosition, pos, 6 * Time.deltaTime);
            sellMenu.transform.localPosition = newMovePos;
        }
    }
}
