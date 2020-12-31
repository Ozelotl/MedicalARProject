//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// screw guide API
/// also switches between visualization and placement behaviour
/// </summary>
public class ScrewGuide : MonoBehaviour
{
    [SerializeField]
    public Transform modelParent;

    public ScrewGuidePlacement placement;
    public ScrewGuideVisualization visualization;
    public Vector3 Direction
    {
        get
        {
            return -transform.up;
        }
    }

    public bool Focused { get { return ScrewGuideCollection.Instance.focusedScrewGuide == this; } }

    public void enterPlacementPhase()
    {
        placement.enabled = true;
        visualization.enabled = false;
    }
    public void enterVisualizationPhase()
    {
        placement.enabled = false;
        visualization.enabled = true;
    }
}
