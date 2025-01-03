using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUpgrade : item
{
    public void Awake()
    {
        int additionalX = 0;
        int additionalY = 0;

        Inventory playerInventory = FindObjectOfType<Inventory>();
        if(playerInventory.GetInventorySize().x > playerInventory.GetInventorySize().y)
        {
            additionalY += 1;
        }
        else
        {
            additionalX += 1;
        }

        playerInventory.AddInventorySize(additionalX, additionalY);

        Destroy(this.gameObject);
    }
}
