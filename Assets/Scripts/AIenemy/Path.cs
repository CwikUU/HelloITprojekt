using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector2[] lookPoints;
    public readonly Line[] turnBoundries;
    public readonly int finishLineIndex;

    public Path(Vector2[] waypoints, Vector2 startPos, float turnDistance)
    {
        lookPoints = waypoints;
        turnBoundries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundries.Length - 1;

        Vector2 previousPoint = startPos;
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundryPoint = (i == finishLineIndex)?currentPoint : currentPoint - directionToCurrentPoint * turnDistance;
            turnBoundries[i] = new Line(turnBoundryPoint, previousPoint - directionToCurrentPoint*turnDistance);
            previousPoint = turnBoundryPoint;

        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector2 point in lookPoints)
        {
            Gizmos.DrawCube(point + Vector2.up,Vector2.one);
        }

        Gizmos.color = Color.white;
        foreach (Line line in turnBoundries)
        {
            line.DrawWithGizmos(10);
        }

    }
}

