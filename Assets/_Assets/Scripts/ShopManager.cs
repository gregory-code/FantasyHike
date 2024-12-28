using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Image sellItemImage;
    [SerializeField] TextMeshProUGUI sellItemPrice;

    [SerializeField] ParticleSystem moneyPoofVFX;

    private SaveManager saveManager;

    [SerializeField] SellMenu sellMenu;
    [SerializeField] Vector3 nonAvaiableSellMenu;
    [SerializeField] Vector3 avaiableSellMenu;
    [SerializeField] Vector3 shownSellMenu;

    bool sellAvailable;
    bool showSell;

    public void Start()
    {
        sellAvailable = true;

        saveManager = FindObjectOfType<SaveManager>();

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
            saveManager.saveData.itemNames.Remove(itemDragging.itemName);
            saveManager.saveData.itemGridPos.Remove(itemDragging.GetGridPos());
            Destroy(itemDragging.gameObject);

            ParticleSystem moneyBurst = Instantiate(moneyPoofVFX, new Vector3(3f,1.5f,0), moneyPoofVFX.transform.rotation);
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[moneyBurst.emission.burstCount];
            bursts[0].count = itemValue;

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
