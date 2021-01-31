//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// responsible for handling screw behaviour during visualization phase
/// </summary>
public class ScrewGuideVisualization : MonoBehaviour
{
    [SerializeField]
    private ScrewGuide _guide;
    private Transform _guideTransform;

    //Used so that a screw where tool is near is visualized
    [SerializeField]
    private InteractableWithTool _interactable;

    //

    //API

    public Vector3 PosGuide
    {
        get
        {
            return _guideTransform.position;
        }
    }

    public float PositionDistance
    {
        get
        {
            return Vector3.Distance(PosGuide, TrackedTool.Instance.TooltipPosition);
        }
    }

    public Vector3 DirGuide
    {
        get
        {
            return _guide.Direction;
        }
    }

    public float DirectionAngleBetween
    {
        get
        { 
            return Mathf.Abs(Vector3.Angle(DirGuide, TrackedTool.Instance.Direction));
        }
    }

    //

    private void Awake()
    {
        _guideTransform = _guide.transform;
    }

    private void Update()
    {
        if (_interactable.Interacting)
        {
            ScrewGuideCollection.Instance.focusedScrewGuide = _guide;
        }
        else if (ScrewGuideCollection.Instance.focusedScrewGuide == _guide)
            ScrewGuideCollection.Instance.focusedScrewGuide = null;
    }

    //

    //interactable needs the correct collider size
    public void transferCollider(BoxCollider collider)
    {
        _interactable.transferCollider(collider);
    }
}
