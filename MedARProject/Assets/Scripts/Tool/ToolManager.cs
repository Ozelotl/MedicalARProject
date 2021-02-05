//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the two different tools and provides API
/// </summary>
public class ToolManager : SingletonMonoMortal<ToolManager>
{
    [System.Serializable]
    public struct Tool
    {
        public TrackedTool _tool;
        public Vuforia.CylinderTargetBehaviour _target;
    }

    [SerializeField]
    private Tool _toolOne;
    [SerializeField]
    private Tool _toolDrill;

    private TrackedTool _activeTool;

    public PhaseManager.Phase AutomaticPhase
    {
        get
        {
            if (_activeTool == _toolOne._tool)
                return PhaseManager.Phase.Position;
            else if (_activeTool.CurrentState == TrackedTool.ToolState.InSpine)
                return PhaseManager.Phase.Depth;
            else
                return PhaseManager.Phase.Orientation;
        }
    }

    //API

    public Vector3 Position { get { return _activeTool.Position; } }
    public Quaternion Rotation { get { return _activeTool.Rotation; } }
    public Vector3 TooltipPosition { get { return _activeTool.TooltipPosition; } }
    public Vector3 TooltopPosition { get { return _activeTool.TooltopPosition; } }

    public Vector3 Direction { get { return _activeTool.Direction; } }

    public float Length { get { return _activeTool.Length; } }
    public RaycastHit? HitSpine { get { return _activeTool.HitSpine; } }

    public bool ToolInsideSpine { get { return _activeTool.toolInsideSpine; } }
    public bool TooltipTouchingSpine { get { return _activeTool.tooltipTouchingSpine; } }

    public TrackedTool.ToolState CurrentState { get { return _activeTool.CurrentState; } }

    //

    protected override void Awake()
    {
        base.Awake();

        _activeTool = _toolOne._tool;
    }

    private void Update()
    {
        if (_toolDrill._tool != null &&
            (_toolDrill._target.CurrentStatus == Vuforia.TrackableBehaviour.Status.TRACKED || _toolDrill._target.CurrentStatus == Vuforia.TrackableBehaviour.Status.DETECTED))
            _activeTool = _toolDrill._tool;
        else
            _activeTool = _toolOne._tool;

        PhaseManager p = PhaseManager.Instance;
        if (p != null && p.AutomaticPhase)
            p.CurrentPhase = AutomaticPhase;
    }
}
