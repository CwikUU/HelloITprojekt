using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Prefab of the enemy to spawn

    private EnemyAIController_Mele proba;
    private GridMana gridMana;

    private void Start()
    {
        proba = FindObjectOfType<EnemyAIController_Mele>();
        gridMana = FindObjectOfType<GridMana>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnEnemy();
        }    
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition;
        Node node;

        // Próbuj znaleŸæ walkable pozycjê (max 50 prób)
        int attempts = 0;
        do
        {
            spawnPosition = proba.GetRoamingPosition();
            node = gridMana.NodeFromWorldPoint(spawnPosition);
            attempts++;
        }
        while (node != null && !node.walkable && attempts < 50);

        if (node != null && node.walkable)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Nie znaleziono wolnego miejsca do spawnu!");
        }
    }
}
