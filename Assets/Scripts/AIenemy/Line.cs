using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    const float verticalLineGradient = 1e5f;

    float gradient;
    float yIntercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradientPerpendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointOnPerpendicular)
    {
        float dx = pointOnLine.x - pointOnPerpendicular.x;
        float dy = pointOnLine.y - pointOnPerpendicular.y;

        if (dx == 0)
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        if (gradientPerpendicular == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }

        yIntercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2 (1, gradient);

        approachSide = false;
        approachSide = GetSide(pointOnPerpendicular);
    }

    bool GetSide(Vector2 point)
    {
        return (point.x - pointOnLine_1.x)*(pointOnLine_2.y - pointOnLine_1.y) > (point.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 point)
    {
       return GetSide(point) != approachSide; 
    }

    public void DrawWithGizmos(float length)
    {
        Vector2 lineDir = new Vector2(1, gradient).normalized;
        Vector2 lineCenter = new Vector2(pointOnLine_1.x, pointOnLine_1.y) + Vector2.up;
        Gizmos.DrawLine(lineCenter - lineDir * length/2f, lineCenter + lineDir * length/2f);
    }
    //https://www.youtube.com/watch?v=NjQjl-ZBXoY&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=8&ab_channel=SebastianLague 16.30
}
