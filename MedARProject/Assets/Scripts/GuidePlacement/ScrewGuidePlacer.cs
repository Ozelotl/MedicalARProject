//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// responsible for starting screw guide placement and handling its events
/// </summary>
public class ScrewGuidePlacer : MonoBehaviour
{
    private void Start()
    {
        ScrewGuideCollection.Instance.enterVisualizationPhase();
    }

    public void newGuide()
    {
        if (ScrewGuideCollection.Instance.focusedScrewGuide == null)
            ScrewGuideCollection.Instance.createScrewGuide("ScrewDummy");
    }

    public void placeGuide()
    { 
        ScrewGuideCollection.Instance.focusedScrewGuide.placement.place();
    }
}
