using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/SaveData")]
public class SaveData : ScriptableObject
{
    public int sizeX = 2; // inventory size
    public int sizeY = 2;

    public int money = 0;

    public List<string> itemIDs = new List<string>();

    public int level = 0;

    public void ClearSaveData()
    {
        sizeX = 2;
        sizeY = 2;

        money = 0;

        itemIDs = new List<string>();

        level = 0;
    }
}