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

    [SerializeField] GameObject[] ShopKeepers;
    private Animator shopKeeperAnimator;

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

    private List<ShopItem> currentShopItems = new List<ShopItem>();

    bool sellAvailable;
    bool showSell;

    private bool bChooseOne;

    public delegate void OnMoneyChanged(int newMoney);
    public event OnMoneyChanged onMoneyChanged;

    public void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        playerInventory = FindObjectOfType<Inventory>();

        foreach (item foundItem in playerInventory.GetItemLibrary())
        {
            if (foundItem.bMonsterDrop)
                continue;

            switch (foundItem.myRariety)
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
                bChooseOne = true;
                GiveSelection(true, commonItems);
                AllowSelling(false);
                break;

            case 3:
                SpawnShopKeeper(0);
                GiveSelection(false, GetCombinedList(commonItems, uncommonItems));
                break;

            default:
                AllowSelling(false);
                break;
        }
    }

    private void SpawnShopKeeper(int which)
    {
        GameObject shopKeeper = Instantiate(ShopKeepers[which], new Vector3(0, 0, 4), ShopKeepers[which].transform.rotation);
        shopKeeperAnimator = shopKeeper.GetComponent<Animator>();

        AllowSelling(true);
        StartCoroutine(FindObjectOfType<OnwardButton>().Show());
    }

    private List<item> GetCombinedList(List<item> list1, List<item> list2)
    {
        List<item> combinedList = list1;
        foreach(item newItem in list2)
        {
            combinedList.Add(newItem);
        }
        return combinedList;
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
                uniqueOption = true;
                for(int x = 0; x < saveManager.saveData.itemIDs.Count; x++)
                {
                    if (newItem.itemName == saveManager.GetIDName(x))
                    {
                        uniqueOption = false;
                    }
                }
                if(uniqueOption == true)
                {
                    itemGroup.Remove(newItem);
                }
            }
            while (uniqueOption == false);

            Transform mainCanvasv = GameObject.FindWithTag("MainCanvas").transform;
            ShopItem newShopItem = Instantiate(shopItemTemplate, mainCanvasv);
            currentShopItems.Add(newShopItem);
            newShopItem.onItemPicked += itemPicked;
            newShopItem.transform.SetSiblingIndex(4);
            newShopItem.transform.localPosition = new Vector3((i * 200) - 200, 100, 0);
            newShopItem.Init(newItem, freeItems);
        }
    }

    private void itemPicked(ShopItem shopItemClicked)
    {
        shopItemClicked.onItemPicked -= itemPicked;
        if(bChooseOne)
        {
            foreach(ShopItem shop in currentShopItems)
            {
                Destroy(shop.gameObject);
            }
            currentShopItems.Clear();
            StartCoroutine(FindObjectOfType<OnwardButton>().Show());
        }

        Inventory playerInventory = FindObjectOfType<Inventory>();
        onMoneyChanged?.Invoke(playerInventory.GetMoney());

        if (shopKeeperAnimator != null)
        {
            shopKeeperAnimator.SetTrigger("approval");
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
            SellItem(itemDragging, itemValue);
        }
    }

    public void SellItem(item itemToSell, int itemValue)
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        //sell it here
        saveManager.saveData.itemIDs.Remove($"{itemToSell.itemName}/{itemToSell.GetGridPos().x}_{itemToSell.GetGridPos().y}/{itemToSell.GetBlankNum()}");
        playerInventory.RegistarItem(itemToSell, false);
        itemToSell.RemoveItem();
        Destroy(itemToSell.gameObject);

        ParticleSystem moneyBurst = Instantiate(moneyPoofVFX, new Vector3(2.4f, 1.3f, 4), moneyPoofVFX.transform.rotation);
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[moneyBurst.emission.burstCount];
        bursts[0].count = itemValue;
        moneyBurst.emission.SetBurst(0, bursts[0]);
        moneyBurst.Play();
        Destroy(moneyBurst, 1);

        playerInventory.SetMoney(playerInventory.GetMoney() + itemValue);
        onMoneyChanged?.Invoke(playerInventory.GetMoney());
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
