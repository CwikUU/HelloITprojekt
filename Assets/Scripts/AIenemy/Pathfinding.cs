using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    GridMana grid;

    private void Awake()
    {
        grid = GetComponent<GridMana>();
    }

    private void Update()
    {
        //FindPatch(seeker.position, target.position); // Wywo�aj funkcj� FindPatch z pozycji szukaj�cego i celu
    }

    IEnumerator FindPatch(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false; // Flaga do sprawdzenia, czy �cie�ka zosta�a znaleziona

        Node startNode = grid.NodeFromWorldPoint(startPos); // Pobierz w�ze� startowy na podstawie pozycji startowej
        Node targetNode = grid.NodeFromWorldPoint(targetPos); // Pobierz w�ze� docelowy na podstawie pozycji docelowej

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); // Lista w�z��w do przeszukania
        HashSet<Node> closedSet = new HashSet<Node>(); // Zbi�r w�z��w ju� przeszukanych

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize); // Inicjalizacja kopca do przechowywania w�z��w do przeszukania
            HashSet<Node> closedSet = new HashSet<Node>(); // Inicjalizacja zbioru do przechowywania w�z��w ju� przeszukanych
            openSet.Add(startNode); // Dodaj w�ze� startowy do listy do przeszukania

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst(); // Pobierz w�ze� o najni�szym koszcie z listy do przeszukania
                closedSet.Add(currentNode); // Dodaj bie��cy w�ze� do zbioru przeszukanych

                if (currentNode == targetNode)
                {
                    pathSuccess = true; // Je�li bie��cy w�ze� jest w�z�em docelowym, oznacz �cie�k� jako znalezion�
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) // Iteruj przez s�siaduj�ce w�z�y
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) // Sprawd�, czy w�ze� jest przechodni i nie zosta� ju� przeszukany
                    {
                        continue; // Je�li nie, pomi� ten w�ze�
                    }

                    int newMovmentCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovmentCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // Sprawd�, czy nowy koszt ruchu do s�siada jest mniejszy ni� dotychczasowy
                    {
                        neighbour.gCost = newMovmentCostToNeighbour; // Ustaw nowy koszt g dla s�siada
                        neighbour.hCost = GetDistance(neighbour, targetNode); // Oblicz koszt h dla s�siada
                        neighbour.parent = currentNode; // Ustaw bie��cy w�ze� jako rodzica s�siada

                        if (!openSet.Contains(neighbour)) // Je�li s�siad nie jest ju� na li�cie do przeszukania
                        {
                            openSet.Add(neighbour); // Dodaj go do listy do przeszukania
                        }
                    }
                }
            }
        }
        // Je�li nie znaleziono �cie�ki, zwr�� pust� tablic�
        yield return null; // Poczekaj na zako�czenie klatki, aby unikn�� zaci�cia gry
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode,targetNode); // Je�li �cie�ka zosta�a znaleziona, retrace path
        }
    }
    
    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>(); // Lista do przechowywania w�z��w �cie�ki
        Node currentNode = endNode; // Rozpocznij od w�z�a docelowego
        
        while (currentNode != startNode) // Dop�ki nie dotrzesz do w�z�a startowego
        {
            path.Add(currentNode); // Dodaj bie��cy w�ze� do listy �cie�ki
            currentNode = currentNode.parent; // Przejd� do rodzica bie��cego w�z�a
        }
        Vector2[] waypoints = SimplifyPath(path); // Upro�� �cie�k� do punkt�w nawigacyjnych
        Array.Reverse(waypoints); // Odwr�� tablic� punkt�w nawigacyjnych, aby zaczyna�a si� od w�z�a startowego
        return waypoints; // Zwr�� tablic� punkt�w nawigacyjnych
    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>(); // Lista do przechowywania punkt�w nawigacyjnych
        Vector2 directionOld = Vector2.zero; // Poprzedni kierunek, pocz�tkowo zerowy
        
        for (int i = 1; i < path.Count; i++) // Iteruj przez ka�dy w�ze� w �cie�ce
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld) // Je�li to pierwszy w�ze� lub odleg�o�� do ostatniego punktu jest wi�ksza ni� 0.1
            {
                waypoints.Add(path[i].worldPosition); // Dodaj bie��cy punkt do listy punkt�w nawigacyjnych
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray(); // Zwr�� tablic� punkt�w nawigacyjnych
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // Oblicz r�nic� w poziomie
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // Oblicz r�nic� w pionie
        if (dstX > dstY) // Je�li r�nica w poziomie jest wi�ksza
        {
            return 14 * dstY + 10 * (dstX - dstY); // Koszt przej�cia przez w�ze�
        }
        return 14 * dstX + 10 * (dstY - dstX); // Koszt przej�cia przez w�ze�
    }
    

}
