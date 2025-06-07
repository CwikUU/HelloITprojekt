using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importing TextMeshPro for UI text handling

public class ArenaSpawnerController : MonoBehaviour
{
    [SerializeField] private TMP_Text mele;
    [SerializeField] private TMP_Text range;
    [SerializeField] private GameObject melePrefab; // Prefab of the enemy to spawn
    [SerializeField] private GameObject rangePrefab; // Prefab of the enemy to spawn
    [SerializeField] private LayerMask unwalkableMask; // Layer mask for unwalkable areas
    [SerializeField] private float howFarX, howFarY; // Distance from the center of the grid to spawn enemies

    private int meleCount = 0; // Counter for melee enemies
    private int rangeCount = 0; // Counter for ranged enemies

    private EnemyAIController_Mele proba;
    private GridMana gridMana;


    private void Start()
    {
        proba = FindObjectOfType<EnemyAIController_Mele>();
        mele.text = "0"; // Initialize melee count UI
        range.text = "0"; // Initialize ranged count UI
    }

    public void SpawnEnemy()
    {
        Vector2 spawnPosition;
        int attempts = 0;
        bool canSpawn = false;
       
        if (meleCount > 0)
        {
            for (int i = 0; i < meleCount; i++)
            {
                attempts = 0;
                canSpawn = false; // Reset canSpawn for each attempt
                while (!canSpawn && attempts < 50)
                {
                    spawnPosition = new Vector2(
                    Random.Range(-howFarX, howFarX),
                    Random.Range(-howFarY, howFarY)
                    );
                    // SprawdŸ, czy w tym miejscu NIE MA kolizji z blokuj¹c¹ warstw¹
                    Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 0.2f, unwalkableMask);
                    canSpawn = (hit == null);
                    attempts++;

                    if (canSpawn)
                    {
                        Instantiate(melePrefab, spawnPosition, Quaternion.identity);
                    }
                }
            }
        }

        if (rangeCount > 0)
        {
            for (int i = 0; i < rangeCount; i++)
            {
                attempts = 0;
                canSpawn = false;
                while (!canSpawn && attempts < 50)
                {
                    spawnPosition = new Vector2(
                    Random.Range(-howFarX, howFarX),
                    Random.Range(-howFarY, howFarY)
                    );
                    // SprawdŸ, czy w tym miejscu NIE MA kolizji z blokuj¹c¹ warstw¹
                    Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 0.2f, unwalkableMask);
                    canSpawn = (hit == null);
                    attempts++;

                    if (canSpawn)
                    {
                        Instantiate(rangePrefab, spawnPosition, Quaternion.identity);
                    }
                }
            }
        }
        meleCount = 0;
        rangeCount = 0; // Reset counts after spawning enemies
        mele.text = "0"; // Reset melee count UI
        range.text = "0"; // Reset ranged count UI
    }

    public void MeleCount(int number)
    {
        meleCount += number; // Increment the melee enemy count
        if (meleCount < 0)
        {
            meleCount = 0; // Ensure count does not go negative
        }
        mele.text = "" +meleCount; // Update the UI text for melee enemies
    }

    public void RangeCount(int number)
    {
        rangeCount += number; // Increment the ranged enemy count
        if (rangeCount < 0)
        {
            rangeCount = 0; // Ensure count does not go negative
        }
        range.text = "" + rangeCount; // Update the UI text for ranged enemies
    }
}
