using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPool : MonoBehaviour
{
    [SerializeField] Vector3 shownPos;
    [SerializeField] List<Transform> spawnPos = new List<Transform>();
    private List<item> lootItems = new List<item>();

    public void RegisterItemToLootPool(item toRegister)
    {
        lootItems.Add(toRegister);
    }

    public Transform GetRandomSpawn()
    {
        int randomIndex = Random.Range(0, spawnPos.Count);
        Transform FoundSpawn = spawnPos[randomIndex];
        spawnPos.RemoveAt(randomIndex);
        return FoundSpawn;
    }

    public IEnumerator Show()
    {
        while (Vector3.Distance(transform.localPosition, shownPos) >= 0.3f)
        {
            yield return new WaitForEndOfFrame();
            Vector3 newMovePos = Vector3.Lerp(transform.localPosition, shownPos, 12 * Time.deltaTime);
            transform.localPosition = newMovePos;
        }
        GiveItems();
    }

    public void GiveItems()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        foreach(item loot in lootItems)
        {
            if (loot == null)
                continue;
            loot.transform.SetParent(playerInventory.transform);
        }
        lootItems.Clear();
    }
}
