using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] Transform enemyPos;

    [SerializeField] Enemy slime;

    private Player player;

    private List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        player = FindObjectOfType<Player>();
        player.onEndTurn += CharacterEndedTurn;
        StartCoroutine(SetupFight());
    }

    private void CharacterEndedTurn(character character)
    {
        if(character.isPlayer == false)
        {
            Enemy enemy = character.GetComponent<Enemy>();
            enemy.hasActed = true;
        }
        NextTurn();
    }

    private void NextTurn()
    {
        List<Enemy> remainingEnemies = new List<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            if(enemy.hasActed == false)
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
                enemy.hasActed = false;
            }
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
