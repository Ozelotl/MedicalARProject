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
    public float screwDepth;

    public ScrewGuidePlacement placement;
    public ScrewGuideVisualization visualization;
    public Vector3 Direction { get { return -transform.up; } }
    public Vector3 EntryPosition { get { return transform.position; } }
    public Vector3 EndPosition { get { return transform.position + Vector3.down*screwDepth; } }

    //Screw that is currently being adjusted by placement
    public bool Focused { get { return ScrewGuideCollection.Instance.focusedScrewGuide == this; } }

    //switch between placement and visualization logic
    public void enterPhase(ScrewGuideCollection.Phase phase)
    {
        placement.gameObject.SetActive(phase == ScrewGuideCollection.Phase.Placement);
        visualization.gameObject.SetActive(phase == ScrewGuideCollection.Phase.Visualization);
    }

    //each screw model can have its own collider dimensions - transfer these to colliders in placement (and later visualization)
    public void transferCollider(BoxCollider collider)
    {
        placement.transferCollider(collider);
        visualization.transferCollider(collider);
    }
}
