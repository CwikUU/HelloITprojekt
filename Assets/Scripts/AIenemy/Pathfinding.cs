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
        //FindPatch(seeker.position, target.position); // Wywo³aj funkcjê FindPatch z pozycji szukaj¹cego i celu
    }

    public Vector2[] FindPatch(Vector2 startPos, Vector2 targetPos)
    {
        Stopwatch stopwatch = new Stopwatch(); // Utwórz nowy obiekt Stopwatch do pomiaru czasu
        stopwatch.Start(); // Rozpocznij pomiar czasu
        Node startNode = grid.NodeFromWorldPoint(startPos); // Pobierz wêze³ startowy na podstawie pozycji startowej
        Node targetNode = grid.NodeFromWorldPoint(targetPos); // Pobierz wêze³ docelowy na podstawie pozycji docelowej

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); // Lista wêz³ów do przeszukania
        HashSet<Node> closedSet = new HashSet<Node>(); // Zbiór wêz³ów ju¿ przeszukanych

        openSet.Add(startNode); // Dodaj wêze³ startowy do listy do przeszukania

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst(); // Pobierz wêze³ o najni¿szym koszcie z listy do przeszukania

            closedSet.Add(currentNode); // Dodaj bie¿¹cy wêze³ do zbioru przeszukanych

            if (currentNode == targetNode)
            {
                stopwatch.Stop(); // Zatrzymaj pomiar czasu
                UnityEngine.Debug.Log("Path found in: " + stopwatch.ElapsedMilliseconds + " ms"); // Wyœwietl czas znalezienia œcie¿ki
                return RetracePath(startNode, targetNode); // Jeœli osi¹gniêto wêze³ docelowy, zakoñcz przeszukiwanie
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) // Iteruj przez s¹siaduj¹ce wêz³y
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) // SprawdŸ, czy wêze³ jest przechodni i nie zosta³ ju¿ przeszukany
                {
                    continue; // Jeœli nie, pomiñ ten wêze³
                }

                int newMovmentCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovmentCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // SprawdŸ, czy nowy koszt ruchu do s¹siada jest mniejszy ni¿ dotychczasowy
                {
                    neighbour.gCost = newMovmentCostToNeighbour; // Ustaw nowy koszt g dla s¹siada
                    neighbour.hCost = GetDistance(neighbour, targetNode); // Oblicz koszt h dla s¹siada
                    neighbour.parent = currentNode; // Ustaw bie¿¹cy wêze³ jako rodzica s¹siada

                    if (!openSet.Contains(neighbour)) // Jeœli s¹siad nie jest ju¿ na liœcie do przeszukania
                    {
                        openSet.Add(neighbour); // Dodaj go do listy do przeszukania
                    }
                }
            }
        }
        // Jeœli nie znaleziono œcie¿ki, zwróæ pust¹ tablicê
        return new Vector2[0];
    }
    
    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>(); // Lista do przechowywania œcie¿ki
        Node currentNode = endNode; // Rozpocznij od wêz³a docelowego

        while (currentNode != startNode) // Dopóki nie osi¹gniêto wêz³a startowego
        {
            path.Add(currentNode); // Dodaj bie¿¹cy wêze³ do œcie¿ki
            currentNode = currentNode.parent; // PrzejdŸ do rodzica bie¿¹cego wêz³a
        }

        
        Vector2[] waypoints = SimplifyPath(path); // Uproœæ œcie¿kê do punktów nawigacyjnych
        Array.Reverse(waypoints); // Odwróæ kolejnoœæ punktów, aby zaczynaæ od wêz³a startowego

        return waypoints;
    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>(); // Lista do przechowywania punktów nawigacyjnych
        Vector2 directionOld = Vector2.zero; // Poprzedni kierunek, pocz¹tkowo zerowy
        for (int i = 1; i < path.Count; i++) // Iteruj przez ka¿dy wêze³ w œcie¿ce
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld) // Jeœli to pierwszy wêze³ lub odleg³oœæ do ostatniego punktu jest wiêksza ni¿ 0.1
            {
                waypoints.Add(path[i].worldPosition); // Dodaj bie¿¹cy punkt do listy punktów nawigacyjnych
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray(); // Zwróæ tablicê punktów nawigacyjnych
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // Oblicz ró¿nicê w poziomie
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // Oblicz ró¿nicê w pionie
        if (dstX > dstY) // Jeœli ró¿nica w poziomie jest wiêksza
        {
            return 14 * dstY + 10 * (dstX - dstY); // Koszt przejœcia przez wêze³
        }
        return 14 * dstX + 10 * (dstY - dstX); // Koszt przejœcia przez wêze³
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //jak zrobic pathfinding
    //1. Stwórz siatkê w której bêdziesz szuka³ œcie¿ki
    //2. Zdefiniuj wêz³y (nodes) na siatce punkty ktore beda przechowywac x y i inne informacje koszty nie koszty
    //3. Stwórz algorytm A* który bêdzie szuka³ najkrótszej œcie¿ki miêdzy dwoma punktami
    //4. Zaimplementuj algorytm w kodzie, który bêdzie iterowa³ przez wêz³y i znajdowa³ najkrótsz¹ œcie¿kê
    //5. Zaimplementuj funkcjê, która bêdzie rysowaæ œcie¿kê na siatce poprzez gizmos or debug.drawline
    //6. Zaimplementuj funkcjê, która bêdzie aktualizowaæ siatkê i wêz³y w czasie rzeczywistym, jeœli zajdzie taka potrzeba
    //7. Zaimplementuj funkcjê, która bêdzie sprawdzaæ kolizje z przeszkodami i aktualizowaæ siatkê

}
