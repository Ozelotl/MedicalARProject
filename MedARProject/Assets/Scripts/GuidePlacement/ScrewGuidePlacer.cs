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
        ScrewGuideCollection.Instance.enterPhase(ScrewGuideCollection.Phase.Placement);
    }

    //called via global speech command on this gameObject
    public void newGuide()
    {
        //Create only if no screw is being adjusted
        //Currently we only have one type of screw that can be placed
        if (ScrewGuideCollection.Instance.focusedScrewGuide == null)
            ScrewGuideCollection.Instance.createScrewGuide("ScrewDummy");
    }

    //called via global speech command on this gameObject
    public void placeGuide()
    {
        if (ScrewGuideCollection.Instance.focusedScrewGuide != null)
            ScrewGuideCollection.Instance.focusedScrewGuide.placement.place();
    }
}
