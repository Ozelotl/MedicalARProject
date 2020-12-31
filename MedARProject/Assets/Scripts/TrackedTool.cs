//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// API for the tracked tool
/// </summary>
public class TrackedTool : SingletonMonoMortal<TrackedTool>
{
    [SerializeField]
    public Transform _tooltip;

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }
    }

    public Vector3 TooltipPosition
    {
        get
        {
            return _tooltip.position;
        }
    }

    public Vector3 Direction
    {
        get
        {
            return -transform.up;
        }
    }
}
