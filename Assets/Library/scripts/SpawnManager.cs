using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    public GameObject[] enemyPrefabs;
    private float spawnRange = 8;

    public int enemyCount;
    int waveNumber = 1;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound = 2;
    // Start is called before the first frame update
    void Start()
    {
        //spawning the powerUp
        spawnPowerUp();

        SpawnEnemyWave(waveNumber);
        
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length; // finds all the Enemy gameObjects
        if(enemyCount == 0) //checking if there are no enemies then spawn new enemies
        {
            waveNumber++;
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }
            
            
            spawnPowerUp();

        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for(int i =0;i <enemiesToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomEnemy], GenerateSpawnPosition(), enemyPrefabs[randomEnemy].transform.rotation);
        }
    }

    void spawnPowerUp()
    {
        int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
        Instantiate(powerUpPrefabs[randomPowerUp], GenerateSpawnPosition(),
            powerUpPrefabs[randomPowerUp].transform.rotation);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosY = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosY);
        return randomPos;
    }

    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;
        if(bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);

        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for(int i =0; i< amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }
    
}
