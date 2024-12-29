using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Screen Transition")]
    [SerializeField] Image screenTransition;
    float transitionValue;
    [SerializeField] Material[] transitionMaterials;

    [SerializeField] Transform playerPos;
    [SerializeField] Transform enemyPos;

    [SerializeField] Enemy slime;

    bool moveToNextScene;

    private SaveManager saveManager;

    private Player player;

    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        player = FindObjectOfType<Player>();
        player.onEndTurn += CharacterEndedTurn;

        moveToNextScene = false;

        FindObjectOfType<OnwardButton>().onOnward += Onward;

        StartCoroutine(SetUpRandomTransition(0, 1));

        switch(saveManager.saveData.level)
        {
            case 1:
                StartCoroutine(SetupFight());
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            saveManager.ResetSaveData();
            Debug.LogWarning("Reset Data");
            SceneManager.LoadScene("StartingScene");
        }
    }

    private void Onward()
    {
        saveManager.saveData.level++;
        moveToNextScene = true;
        StartCoroutine(SetUpRandomTransition(1, 0));
    }

    private IEnumerator SetUpRandomTransition(int defaultValue, int newValue)
    {
        int randomTransition = Random.Range(0, 3);
        screenTransition.material = transitionMaterials[randomTransition];
        transitionValue = defaultValue;
        transitionMaterials[randomTransition].SetFloat("_Transition", defaultValue);

        yield return new WaitForSeconds(0.5f);

        if(newValue == 1)
        {
            while (transitionValue < newValue)
            {
                yield return new WaitForEndOfFrame();
                transitionValue += Time.deltaTime;
                transitionMaterials[randomTransition].SetFloat("_Transition", transitionValue);
            }
        }
        else
        {
            while (transitionValue > newValue)
            {
                yield return new WaitForEndOfFrame();
                transitionValue -= Time.deltaTime;
                transitionMaterials[randomTransition].SetFloat("_Transition", transitionValue);
            }
        }

        saveManager.Save();
        yield return new WaitForSeconds(0.2f);
        if(moveToNextScene)
        {
            MoveToNextLevel();
        }
    }

    private void MoveToNextLevel()
    {
        SceneManager.LoadScene("ForestScene");
    }

    private void CharacterEndedTurn(character character)
    {
        if(character.isPlayer == false)
        {
            Enemy enemy = character.GetComponent<Enemy>();
            enemy.SetHasActed(true);
        }
        NextTurn();
    }

    private void NextTurn()
    {
        List<Enemy> remainingEnemies = new List<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if(enemy.HasActed() == false)
            {
                remainingEnemies.Add(enemy);
            }
        }

        if(remainingEnemies.Count >= 1)
        {
            remainingEnemies[Random.Range(0, remainingEnemies.Count)].Attack(player);
        }
        else
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.SetHasActed(false);
            }

            if(enemies.Count > 0)
                player.ShowActions();
        }
    }

    public void RemoveEnemy(Enemy enemyToRemove)
    {
        enemies.Remove(enemyToRemove);
        if(enemies.Count <= 0)
        {
            StartCoroutine(EndFight());
        }
    }

    private IEnumerator EndFight()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FindObjectOfType<OnwardButton>().Show());
        StartCoroutine(FindObjectOfType<LootPool>().Show());
    }

    private IEnumerator SetupFight()
    {
        enemies.Clear();

        Vector3 spawnPos = enemyPos.position;
        spawnPos.x += 10f;

        SpawnEnemy(spawnPos);

        yield return new WaitForSeconds(3f);

        player.ShowActions();
    }

    private void SpawnEnemy(Vector3 spawnPos)
    {
        Enemy newEnemy = Instantiate(slime, spawnPos, Quaternion.identity);
        newEnemy.Init(this);
        newEnemy.onEndTurn += CharacterEndedTurn;
        enemies.Add(newEnemy);
        StartCoroutine(newEnemy.MoveTo(enemyPos));
    }
}
