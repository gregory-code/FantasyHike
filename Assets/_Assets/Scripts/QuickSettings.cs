using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickSettings : MonoBehaviour
{
    public void Retart()
    {
        FindObjectOfType<SaveManager>().ResetSaveData();
        SceneManager.LoadScene("StartingScene");
    }

    public void RetryBattle()
    {
        SceneManager.LoadScene("ForestScene");
    }
}
