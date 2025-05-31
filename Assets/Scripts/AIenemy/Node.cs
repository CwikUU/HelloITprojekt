using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable; // Czy wêze³ jest
    public Vector3 worldPosition; // Pozycja wêz³a w przestrzeni 3D

    public Node(bool _walkable, Vector3 _worldPosition)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
    }

}
