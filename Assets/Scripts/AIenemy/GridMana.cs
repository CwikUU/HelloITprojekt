using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMana : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask; // Maska warstwy, która okreœla, które obiekty s¹ nieprzechodnie
    public Vector2 gridWorldSize; // Rozmiar siatki w œwiecie
    public float nodeRadius; // Promieñ wêz³a
    Node[,] grid; // Tablica wêz³ów

    private float nodeDiameter; // Œrednica wêz³a
    private int gridSizeX, gridSizeY; // Rozmiar siatki w wêz³ach

    private void Start()
    {
        nodeDiameter = nodeRadius * 2; // Obliczenie œrednicy wêz³a
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Obliczenie rozmiaru siatki w poziomie
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Obliczenie rozmiaru siatki w pionie
        CreateGrid(); // Utworzenie siatki
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // Inicjalizacja tablicy wêz³ów
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; // Obliczenie lewego dolnego rogu siatki

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius); // Obliczenie pozycji wêz³a w œwiecie
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask); // Sprawdzenie, czy wêze³ jest przechodni (czy nie koliduje z obiektami nieprzechodnimi)
                grid[x, y] = new Node(walkable, worldPoint); // Utworzenie nowego wêz³a i przypisanie go do tablicy
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Obliczenie wspó³rzêdnych wêz³a na podstawie pozycji w œwiecie
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX); // Ograniczenie wartoœci do zakresu [0, 1]
        percentY = Mathf.Clamp01(percentY); // Ograniczenie wartoœci do zakresu [0, 1]

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); // Obliczenie indeksu wêz³a w poziomie
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY); // Obliczenie indeksu wêz³a w pionie

        return grid[x, y]; // Zwrócenie wêz³a z tablicy
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Rysuj ramkê siatki w przestrzeni 2D (X, Y)
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0.1f));

        if (grid != null)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = n.walkable ? Color.white : Color.red;
                    if (n == playerNode)
                    {
                        Gizmos.color = Color.green; // Zmiana koloru wêz³a gracza na zielony
                    }
                    // Rysuj wêz³y jako p³askie kwadraty w p³aszczyŸnie XY
                    Gizmos.DrawCube(
                        new Vector3(n.worldPosition.x, n.worldPosition.y, 0f),
                        new Vector3(nodeDiameter - 0.1f, nodeDiameter - 0.1f, 0.05f)
                    );
                }
            }
        }
    }
}


