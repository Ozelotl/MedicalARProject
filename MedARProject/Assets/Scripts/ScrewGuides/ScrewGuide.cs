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

    //Screw that is currently being adjusted by placement or viszalized by visualization
    public bool Focused { get { return ScrewGuideCollection.Instance.focusedScrewGuide == this; } }

    //switch between placement and visualization logic
    public void enterPhase(ScrewGuideCollection.Phase phase)
    {
        placement.enabled = phase == ScrewGuideCollection.Phase.Placement;
        visualization.enabled = phase == ScrewGuideCollection.Phase.Visualization;
    }

    //each screw model can have its own collider dimensions - transfer these to colliders in placement (and later visualization)
    public void transferCollider(BoxCollider collider)
    {
        placement._collider.center = collider.center;
        placement._collider.size = collider.size;
    }
}
