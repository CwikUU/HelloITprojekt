using System.Collections;
using System.Collections.Generic;
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
        FindPatch(seeker.position, target.position); // Wywo�aj funkcj� FindPatch z pozycji szukaj�cego i celu
    }
    void  FindPatch(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos); // Pobierz w�ze� startowy na podstawie pozycji startowej
        Node targetNode = grid.NodeFromWorldPoint(targetPos); // Pobierz w�ze� docelowy na podstawie pozycji docelowej

        List<Node> openSet = new List<Node>(); // Lista w�z��w do przeszukania
        HashSet<Node> closedSet = new HashSet<Node>(); // Zbi�r w�z��w ju� przeszukanych

        openSet.Add(startNode); // Dodaj w�ze� startowy do listy do przeszukania

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i]; // Znajd� w�ze� o najni�szym koszcie f
                }
            }

            openSet.Remove(currentNode); // Usu� bie��cy w�ze� z listy do przeszukania
            closedSet.Add(currentNode); // Dodaj bie��cy w�ze� do zbioru przeszukanych

            if (currentNode == targetNode){
                RetracePath(startNode, targetNode); // Je�li osi�gni�to w�ze� docelowy, wywo�aj funkcj� do �ledzenia �cie�ki
                return; // Je�li osi�gni�to w�ze� docelowy, zako�cz przeszukiwanie
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
    
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>(); // Lista do przechowywania �cie�ki
        Node currentNode = endNode; // Rozpocznij od w�z�a docelowego

        while (currentNode != startNode) // Dop�ki nie osi�gni�to w�z�a startowego
        {
            path.Add(currentNode); // Dodaj bie��cy w�ze� do �cie�ki
            currentNode = currentNode.parent; // Przejd� do rodzica bie��cego w�z�a
        }
        path.Reverse(); // Odwr�� �cie�k�, aby uzyska� poprawny kierunek od startu do ko�ca

        grid.path = path; // Przypisz znalezion� �cie�k� do siatki
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
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //jak zrobic pathfinding
    //1. Stw�rz siatk� w kt�rej b�dziesz szuka� �cie�ki
    //2. Zdefiniuj w�z�y (nodes) na siatce punkty ktore beda przechowywac x y i inne informacje koszty nie koszty
    //3. Stw�rz algorytm A* kt�ry b�dzie szuka� najkr�tszej �cie�ki mi�dzy dwoma punktami
    //4. Zaimplementuj algorytm w kodzie, kt�ry b�dzie iterowa� przez w�z�y i znajdowa� najkr�tsz� �cie�k�
    //5. Zaimplementuj funkcj�, kt�ra b�dzie rysowa� �cie�k� na siatce poprzez gizmos or debug.drawline
    //6. Zaimplementuj funkcj�, kt�ra b�dzie aktualizowa� siatk� i w�z�y w czasie rzeczywistym, je�li zajdzie taka potrzeba
    //7. Zaimplementuj funkcj�, kt�ra b�dzie sprawdza� kolizje z przeszkodami i aktualizowa� siatk�

}
