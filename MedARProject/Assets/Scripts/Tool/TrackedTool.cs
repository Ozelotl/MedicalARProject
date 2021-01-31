//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// API for the tracked tool
/// </summary>
public class TrackedTool : SingletonMonoMortal<TrackedTool>
{
    public enum ToolState
    { 
        OutsideSpine,
        NearSpine,
        OnSpine,
        InSpine
    }

    [SerializeField]
    private Transform _tooltip;
    [SerializeField]
    private Transform _tooltop;

    //API

    public Vector3 Position { get { return transform.position; } }
    public Quaternion Rotation { get { return transform.rotation; } }
    public Vector3 TooltipPosition { get { return _tooltip.position; } }
    public Vector3 TooltopPosition { get { return _tooltop.position; } }

    public Vector3 Direction { get { return -transform.up.normalized; } }

    public float Length { get { return Vector3.Distance(TooltipPosition, TooltopPosition); } }

    private RaycastHit? _hitSpine;
    public RaycastHit? HitSpine { get { return _hitSpine; } }

    public bool toolInsideSpine;
    public bool tooltipTouchingSpine;

    private ToolState _currentState;
    public ToolState CurrentState { get { return _currentState; } }

    //

    //Project unto spine model
    private void Update()
    {
        setCurrentState();
        castRayOntoSpine();
    }

    private void castRayOntoSpine()
    {
        _hitSpine = null;

        if (!toolInsideSpine)
        {
            RaycastHit hit;
            if (Physics.Raycast(Position, Direction, out hit, 1f, LayerMask.GetMask("Spine")))
                _hitSpine = hit;
        }
    }

    private void setCurrentState()
    {
        if (toolInsideSpine)
            _currentState = ToolState.InSpine;
        else if (tooltipTouchingSpine)
            _currentState = ToolState.OnSpine;
        else
            _currentState = ToolState.OutsideSpine;
    }
}
