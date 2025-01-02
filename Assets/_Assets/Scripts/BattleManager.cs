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
    private GameObject[] enemyPos;

    [SerializeField] Enemy[] enemyLibrary;

    bool moveToNextScene;

    private SaveManager saveManager;

    private Player player;

    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        enemyPos = GameObject.FindGameObjectsWithTag("enemyPos");

        saveManager = FindObjectOfType<SaveManager>();
        player = FindObjectOfType<Player>();
        player.onEndTurn += CharacterEndedTurn;

        moveToNextScene = false;

        FindObjectOfType<OnwardButton>().onOnward += Onward;

        StartCoroutine(SetUpRandomTransition(0, 1));

        switch(saveManager.saveData.level)
        {

            case 0:
            case 3:
                //No fights
                break;


            default:
                StartCoroutine(SetupFight());
                break;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartOver();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("ForestScene");
        }
    }

    public void StartOver()
    {
        saveManager.ResetSaveData();
        SceneManager.LoadScene("StartingScene");
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
            remainingEnemies[Random.Range(0, remainingEnemies.Count)].EnemiesTurn(player);
        }
        else
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.SetHasActed(false);
            }

            if(enemies.Count > 0 && player.currentHealth >= 1)
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

    public Enemy[] GetAllEnemies()
    {
        return enemies.ToArray();
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

        int totalPower = saveManager.saveData.level;

        for(int i = 0; i < enemyPos.Length; i++)
        {
            if (totalPower == 0)
                break;

            List<Enemy> enemyPool = new List<Enemy>();
            foreach(Enemy possbileEnemy in enemyLibrary)
            {
                if (possbileEnemy.power <= totalPower)
                    enemyPool.Add(possbileEnemy);
            }

            int randomEnemy = Random.Range(0, enemyPool.Count);
            totalPower -= enemyPool[randomEnemy].power;

            int addedPower = 0;
            if (totalPower > 0)
            {
                addedPower = (i == enemyPos.Length - 1) ? totalPower : Random.Range(0, totalPower + 1);
                totalPower -= addedPower;
            }

            GameObject pos = enemyPos[i];
            SpawnEnemy(enemyPool[randomEnemy], addedPower, pos);

            yield return new WaitForSeconds(1f);
        }

        player.ShowActions();
    }

    private void SpawnEnemy(Enemy enemyPrefab, int upgradeLevel, GameObject pos)
    {
        Vector3 actualPos = pos.transform.position;
        actualPos.x += 10.0f;

        Enemy newEnemy = Instantiate(enemyPrefab, actualPos, Quaternion.identity);
        newEnemy.Init(this, upgradeLevel, pos);
        newEnemy.onEndTurn += CharacterEndedTurn;
        enemies.Add(newEnemy);
        StartCoroutine(newEnemy.MoveTo(pos.transform));
    }
}
