using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable; // Czy w�ze� jest
    public Vector3 worldPosition; // Pozycja w�z�a w przestrzeni 3D
    public int gridX; // Indeks w�z�a w siatce w poziomie
    public int gridY; // Indeks w�z�a w siatce w pionie

    public int gCost;
    public int hCost; // Koszty g i h dla algorytmu A*
    public Node parent; // Rodzic w�z�a, u�ywany do �ledzenia �cie�ki
    int heapIndex; // Indeks w�z�a w kopcu, u�ywany do szybkiego dost�pu

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

    public int HeapIndex 
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value; // Ustaw indeks kopca dla w�z�a
        }
    } // Indeks w�z�a w kopcu, u�ywany do szybkiego dost�pu

    public int CompareTo(Node NodeToCompare)
    {
        int compare = fCost.CompareTo(NodeToCompare.fCost); // Por�wnaj koszty f w�z��w
        if ( compare == 0)
        {
            compare = hCost.CompareTo(NodeToCompare.hCost); // Je�li koszty f s� r�wne, por�wnaj koszty h
        }
        return -compare; // Zwr�� warto�� por�wnania, aby uporz�dkowa� w�z�y w kopcu (mniejsze koszty na pocz�tku)
    }
}
