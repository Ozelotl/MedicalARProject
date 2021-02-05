//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// API for the tracked tool
/// </summary>
public class TrackedTool: MonoBehaviour
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

    public struct ToolModel
    {
        public Transform trans;
        public Material[] mats;
    }
    private List<ToolModel> _liToolModels;

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

    private void Start()
    {
        Renderer[] ren = GetComponentsInChildren<Renderer>();
        _liToolModels = new List<ToolModel>();
        for (int i = 0; i < ren.Length; i++)
        {
            ToolModel t = new ToolModel();
            t.trans = ren[i].transform;
            t.mats = ren[i].materials;
            _liToolModels.Add(t);
        }
    }

    //Project unto spine model
    private void Update()
    {
        setCurrentState();
        castRayOntoSpine();
    }

    private void castRayOntoSpine()
    {
        _hitSpine = null;

        RaycastHit hit;
        if (Physics.Raycast(Position, Direction, out hit, 1f, LayerMask.GetMask("Spine")))
            _hitSpine = hit;

        for (int i = 0; i < _liToolModels.Count; i++)
        {
            ToolModel t = _liToolModels[i];

            float yEntryPoint = float.MinValue;
            if (_hitSpine != null)
                yEntryPoint = t.trans.InverseTransformPoint(_hitSpine.Value.point).z;

            for (int k = 0; k < t.mats.Length; k++)
                t.mats[k].SetFloat("_ZEntryPoint", yEntryPoint);
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
