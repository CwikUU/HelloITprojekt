using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMana : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask; // Maska warstwy, kt�ra okre�la, kt�re obiekty s� nieprzechodnie
    public Vector2 gridWorldSize; // Rozmiar siatki w �wiecie
    public float nodeRadius; // Promie� w�z�a
    Node[,] grid; // Tablica w�z��w

    private float nodeDiameter; // �rednica w�z�a
    private int gridSizeX, gridSizeY; // Rozmiar siatki w w�z�ach

    private void Start()
    {
        nodeDiameter = nodeRadius * 2; // Obliczenie �rednicy w�z�a
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // Obliczenie rozmiaru siatki w poziomie
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Obliczenie rozmiaru siatki w pionie
        CreateGrid(); // Utworzenie siatki
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // Inicjalizacja tablicy w�z��w
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; // Obliczenie lewego dolnego rogu siatki

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius); // Obliczenie pozycji w�z�a w �wiecie
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask); // Sprawdzenie, czy w�ze� jest przechodni (czy nie koliduje z obiektami nieprzechodnimi)
                grid[x, y] = new Node(walkable, worldPoint); // Utworzenie nowego w�z�a i przypisanie go do tablicy
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Obliczenie wsp�rz�dnych w�z�a na podstawie pozycji w �wiecie
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX); // Ograniczenie warto�ci do zakresu [0, 1]
        percentY = Mathf.Clamp01(percentY); // Ograniczenie warto�ci do zakresu [0, 1]

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); // Obliczenie indeksu w�z�a w poziomie
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY); // Obliczenie indeksu w�z�a w pionie

        return grid[x, y]; // Zwr�cenie w�z�a z tablicy
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Rysuj ramk� siatki w przestrzeni 2D (X, Y)
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
                        Gizmos.color = Color.green; // Zmiana koloru w�z�a gracza na zielony
                    }
                    // Rysuj w�z�y jako p�askie kwadraty w p�aszczy�nie XY
                    Gizmos.DrawCube(
                        new Vector3(n.worldPosition.x, n.worldPosition.y, 0f),
                        new Vector3(nodeDiameter - 0.1f, nodeDiameter - 0.1f, 0.05f)
                    );
                }
            }
        }
    }
}


