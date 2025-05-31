using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable; // Czy wêze³ jest
    public Vector3 worldPosition; // Pozycja wêz³a w przestrzeni 3D
    public int gridX; // Indeks wêz³a w siatce w poziomie
    public int gridY; // Indeks wêz³a w siatce w pionie

    public int gCost;
    public int hCost; // Koszty g i h dla algorytmu A*
    public Node parent; // Rodzic wêz³a, u¿ywany do œledzenia œcie¿ki

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }

    public int fCost 
    {
        get {
            return gCost + hCost;
        }
    } // Ca³kowity koszt przejœcia przez wêze³
}
