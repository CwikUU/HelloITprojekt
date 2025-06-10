using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaveControler : MonoBehaviour
{
    [SerializeField] private TMP_Text waveNr;
    [Header("Enemy")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangeEnemyPrefab;
    [SerializeField] private GameObject giantEnemyPrefab;
    [Header("Wave 1")]
    [SerializeField] private float waveTimeI;
    [SerializeField] private int meleAmountI;
    [SerializeField] private int rangeAmountI;
    [SerializeField] private int giantAmountI;
    [Header("Wave 2")]
    [SerializeField] private float waveTimeII;
    [SerializeField] private int meleAmountII;
    [SerializeField] private int rangeAmountII;
    [SerializeField] private int giantAmountII;
    [Header("Wave 3")]
    [SerializeField] private float waveTimeIII;
    [SerializeField] private int meleAmountIII;
    [SerializeField] private int rangeAmountIII;
    [SerializeField] private int giantAmountIII;

    private float timer;
    private int currentWave = 0;
    private float xRange;
    private float yRange;
    private bool working;

    private ArenaSpawnerController arenaSpawner;
    private GameObject[] enemies;


    // Start is called before the first frame update
    void Start()
    {
        arenaSpawner = FindObjectOfType<ArenaSpawnerController>();
        xRange = arenaSpawner.howFarX;
        yRange = arenaSpawner.howFarY;
        working = false;
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (working)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (enemies.Length == 0)
                {
                    timer = 0; // Reset timer if enemies are still present
                }
            }
            else
            {
                StartWave();
            }
        }
    }

    public void StartWave()
    {
        if (currentWave == 0)
        {
            SpawnEnemies(meleeEnemyPrefab, meleAmountI);
            SpawnEnemies(rangeEnemyPrefab, rangeAmountI);
            SpawnEnemies(giantEnemyPrefab, giantAmountI);
            timer = waveTimeI;
            currentWave++;
            working = true;
            waveNr.text = "Wave: " + currentWave; 
        }
        else if (currentWave == 1)
        {
            SpawnEnemies(meleeEnemyPrefab, meleAmountII);
            SpawnEnemies(rangeEnemyPrefab, rangeAmountII);
            SpawnEnemies(giantEnemyPrefab, giantAmountII);
            timer = waveTimeII;
            currentWave++;
            waveNr.text = "Wave: " + currentWave;
        }
        else if (currentWave == 2)
        {
            SpawnEnemies(meleeEnemyPrefab, meleAmountIII);
            SpawnEnemies(rangeEnemyPrefab, rangeAmountIII);
            SpawnEnemies(giantEnemyPrefab, giantAmountIII);
            timer = waveTimeIII;
            currentWave++;
            waveNr.text = "Wave: " + currentWave;
        }
        else if (currentWave == 3)
        {
            Debug.Log("All waves completed!");
            // Optionally reset or stop the wave system
            currentWave = 0; // Reset to start over if desired
            timer = 0; // Stop the timer
            working = false; // Stop the wave spawning
            waveNr.text = "";
        }
        
    }


    private void SpawnEnemies(GameObject enemyPrefab, int amount)
    {
        Vector2 spawnPosition;
        bool canSpawn = false;

        for (int i = 0; i < amount; i++)
        {
            canSpawn = false;
            while (!canSpawn)
            {
                spawnPosition = new Vector2(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange));
                Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 0.2f, arenaSpawner.unwalkableMask);
                canSpawn = (hit == null);
                if (canSpawn)
                {
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }
}
