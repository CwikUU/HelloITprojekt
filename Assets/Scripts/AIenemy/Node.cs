using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable; // Czy w�ze� jest
    public Vector3 worldPosition; // Pozycja w�z�a w przestrzeni 3D
    public int gridX; // Indeks w�z�a w siatce w poziomie
    public int gridY; // Indeks w�z�a w siatce w pionie

    public int gCost;
    public int hCost; // Koszty g i h dla algorytmu A*
    public Node parent; // Rodzic w�z�a, u�ywany do �ledzenia �cie�ki

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
    } // Ca�kowity koszt przej�cia przez w�ze�
}
