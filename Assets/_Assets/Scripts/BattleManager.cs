using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private SaveManager saveManager;

    private Player player;

    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        player = FindObjectOfType<Player>();
        player.onEndTurn += CharacterEndedTurn;

        StartCoroutine(SetUpRandomTransition());

        switch(saveManager.saveData.level)
        {
            case 0:
                StartCoroutine(SetupFight());
                break;

            default:
                break;
        }
    }

    private IEnumerator SetUpRandomTransition()
    {
        int randomTransition = Random.Range(0, 3);
        screenTransition.material = transitionMaterials[randomTransition];
        transitionValue = 0;
        transitionMaterials[randomTransition].SetFloat("_Transition", 0);

        while(transitionValue < 1)
        {
            yield return new WaitForEndOfFrame();
            transitionValue += Time.deltaTime;
            transitionMaterials[randomTransition].SetFloat("_Transition", transitionValue);
        }
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
            Debug.Log("You beat the wave");
            //Next game
        }
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
