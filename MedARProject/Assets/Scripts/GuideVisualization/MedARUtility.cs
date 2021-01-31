//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility
/// </summary>
public class MedARUtility
{
    public static string unitsToText(float units, float mmPerUnit)
    {
        float mm = units * mmPerUnit;
        if (Mathf.Abs(mm) > 10)
            return (mm / 10.0f).ToString("0.0") + " cm";
        else
            return mm.ToString("0.0") + " mm";
    }

    public static void DrawLineRendererFromTo(LineRenderer lr, Vector3 from, Vector3 to, float width)
    {
        DrawLineRendererFromTo(lr, from, (to - from).normalized, Vector3.Distance(from, to), width);
    }

    public static void DrawLineRendererFromTo(LineRenderer lr, Vector3 start, Vector3 dir, float dist, float width)
    {
        List<Vector3> liPos = new List<Vector3>();

        Vector3 dirN = dir.normalized;
        float distCur = 0f;

        //set segments with length equal to width to keep texture from being squashed
        while (distCur < dist)
        {
            int count = liPos.Count;
            liPos.Add(start + count * width * dirN);
            distCur += width;
        }
        liPos.Add(start + liPos.Count * width * dirN);

        lr.positionCount = liPos.Count;
        lr.SetPositions(liPos.ToArray());
    }

    //Intersection between 2 3D Line segments (From Unity Community Wiki)
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Approximately(planarFactor, 0f) &&
            !Mathf.Approximately(crossVec1and2.sqrMagnitude, 0f))
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }
}
