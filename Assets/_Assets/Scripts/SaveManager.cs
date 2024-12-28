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

    public void ResetSaveData()
    {
        if (saveData != null)
        {

            Debug.Log("Save data reset to default values.");
            Save();
        }
    }
}
