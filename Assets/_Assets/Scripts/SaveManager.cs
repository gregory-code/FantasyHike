using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;
    private string savePath;

    [SerializeField] bool showPath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");

        if(showPath)
            Debug.Log(savePath);

        Load();
    }

    public void Save()
    {
        if (saveData == null)
        {
            Debug.LogError("SaveData ScriptableObject is not assigned.");
            return;
        }

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(savePath, json);
        Debug.Log($"Data saved to {savePath}");
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            JsonUtility.FromJsonOverwrite(json, saveData);
            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating a new one.");
            Save();
        }
    }

    public string GetIDName(int ID)
    {
        if (string.IsNullOrEmpty(saveData.itemIDs[ID]))
            return string.Empty;

        string[] parts = saveData.itemIDs[ID].Split('/');
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    public Vector2 GetIDGridPos(int ID)
    {
        if (string.IsNullOrEmpty(saveData.itemIDs[ID]))
            return Vector2.zero;

        string[] parts = saveData.itemIDs[ID].Split('/');
        if (parts.Length < 2)
            return Vector2.zero;

        string middlePart = parts[1]; // "3_2"
        string[] coordinates = middlePart.Split('_');

        if (coordinates.Length == 2 &&
            float.TryParse(coordinates[0], out float x) &&
            float.TryParse(coordinates[1], out float y))
        {
            return new Vector2(x, y);
        }

        return Vector2.zero; // Return (0, 0) if parsing fails
    }

    public int GetIDRotation(int ID)
    {
        if (string.IsNullOrEmpty(saveData.itemIDs[ID]))
            return 0;

        string[] parts = saveData.itemIDs[ID].Split('/');
        if (parts.Length < 3)
            return 0;

        if (int.TryParse(parts[2], out int result))
        {
            return result;
        }

        return 0; // Return 0 if parsing fails
    }

    public void ResetSaveData()
    {
        if (saveData != null)
        {

            Debug.Log("Save data reset to default values.");
            Save();
        }
    }
}
