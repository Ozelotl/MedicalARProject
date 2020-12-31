//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// responsible for handling screw behaviour during placement phase
/// </summary>
public class ScrewGuidePlacement : MonoBehaviour
{
    [SerializeField]
    private ScrewGuide _guide;

    private void OnDisable()
    {
        place(); //so that we don't have dangling screws in visualization phase
    }

//

    private void Update()
    {
        //set screw guide to be attached to tooltip if it is the one being adjusted
        if (_guide.Focused)
        {
            _guide.transform.position = TrackedTool.Instance.TooltipPosition;
            _guide.transform.rotation = TrackedTool.Instance.Rotation;
        }
    }

    public void place()
    {
        //stop adjusting this guide
        if (_guide.Focused)
            ScrewGuideCollection.Instance.focusedScrewGuide = null;
    }
}
