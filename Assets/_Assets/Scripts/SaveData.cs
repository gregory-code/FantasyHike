using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/SaveData")]
public class SaveData : ScriptableObject
{
    public int sizeX = 3; // inventory size
    public int sizeY = 3;

    public int money = 0;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public int currentMana = 3;
    public int maxMana = 3;

    public List<string> itemIDs = new List<string>();

    public int level = 0;

    public void ClearSaveData()
    {
        sizeX = 3;
        sizeY = 3;

        money = 0;
        maxHealth = 10;
        currentHealth = maxHealth;

        maxMana = 3;
        currentMana = maxMana;

        itemIDs = new List<string>();

        level = 0;
    }
}