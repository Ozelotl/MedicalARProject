//Stella

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// responsible for starting screw guide visualization and handling its events
/// </summary>
public class ScrewGuideVisualizer : MonoBehaviour
{
    private void Start()
    {
        ScrewGuideCollection.Instance.enterPhase(ScrewGuideCollection.Phase.Visualization);
    }

    private void Update()
    {
        //focusNearestScrewGuide();
    }

    //Currently unused - focus guide where tool is in collider instead
    //private void focusNearestScrewGuide()
    //{
    //    //calculate screw that should be visualized - currently the one nearest the tooltip

    //    Vector3 tooltipPos = TrackedTool.Instance.TooltipPosition;
    //    ReadOnlyCollection<ScrewGuide> liGuides = ScrewGuideCollection.Instance.liGuidesReadonly;

    //    int screwCount = liGuides.Count;
    //    if (screwCount == 0)
    //        return;

    //    ScrewGuide guideNearest = null;
    //    float distanceCur = float.MaxValue;
    //    for (int i = 0; i < screwCount; i++)
    //    {
    //        ScrewGuide guideToEvaluate = liGuides[i];
    //        float distanceToEvaluate = Vector3.Distance(tooltipPos, guideToEvaluate.transform.position);

    //        if (distanceToEvaluate < distanceCur)
    //        {
    //            guideNearest = guideToEvaluate;
    //            distanceCur = distanceToEvaluate;
    //        }
    //    }

    //    ScrewGuideCollection.Instance.focusedScrewGuide = guideNearest;
    //}
}
