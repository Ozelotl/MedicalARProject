//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In Visualize Position Phase:
///     Show lines for tool and guide, tool line green if angle correct, else more and more red
///     lines always drawn on top, semi transparent for parts in spine
///     lines drawn until intersection point if one exists or as long as tool and screw if not
/// </summary>
public class VisualizeOrientation : MonoBehaviour
{

    #region fields

    //Settings

    [Header("Settings")]
    [SerializeField]
    private float _lineWidth;
    [SerializeField]
    private float _lineMargin; //Lines will be drawn a bit longer than e.g. intersection point
    [SerializeField]
    private float _angleMax; //everything larger than this angle is red, between this ant 0 lerp
    
    //References

    [Space]
    [SerializeField]
    private GameObject _linrendererParent;

    [Space]
    [SerializeField]
    private LineRenderer _lineRendererToolOutsideSpine;
    [SerializeField]
    private LineRenderer _lineRendererToolInsideSpine;
    [SerializeField]
    private LineRenderer _lineRendererGuideOutsideSpine;
    [SerializeField]
    private LineRenderer _lineRendererGuideInsideSpine;

    [Space]
    [SerializeField]
    private Color _colorLineToolOutsideSpineCorrect;
    [SerializeField]
    private Color _colorLineToolInsideSpineCorrect;
    [SerializeField]
    private Color _colorLineToolOutsideSpineIncorrect;
    [SerializeField]
    private Color _colorLineToolInsideSpineIncorrect;

    [SerializeField]
    private Color _colorLineGuideOutsideSpine;
    [SerializeField]
    private Color _colorLineGuideInsideSpine;

    private Material _matLRToolOutsideSpine;
    private Material _matLRToolInsideSpine;
    #endregion

    //

    private void Start()
    {
        //LineRenderer colors don't support alpha, but our material does, so set it here
        _matLRToolOutsideSpine = _lineRendererToolOutsideSpine.GetComponent<Renderer>().material;
        _matLRToolInsideSpine = _lineRendererToolInsideSpine.GetComponent<Renderer>().material;
        _matLRToolOutsideSpine.color = _colorLineToolOutsideSpineIncorrect;
        _matLRToolInsideSpine.color = _colorLineToolInsideSpineIncorrect;

        _lineRendererGuideOutsideSpine.GetComponent<Renderer>().material.color = _colorLineGuideOutsideSpine;
        _lineRendererGuideInsideSpine.GetComponent<Renderer>().material.color = _colorLineGuideInsideSpine;

        //Set line width here as easier, additionally, the curves themselves are still supported like this
        _lineRendererToolOutsideSpine.widthMultiplier = _lineWidth;
        _lineRendererToolInsideSpine.widthMultiplier = _lineWidth;
        _lineRendererGuideOutsideSpine.widthMultiplier = _lineWidth;
        _lineRendererGuideInsideSpine.widthMultiplier = _lineWidth;
    }

    private void Update()
    {
        ScrewGuide guide = ScrewGuideCollection.Instance.focusedScrewGuide;
        if (guide == null)
        {
            _linrendererParent.SetActive(false);
        }
        else
        {
            TrackedTool tool = TrackedTool.Instance;
            float angle = Vector3.Angle(tool.Direction, guide.Direction);
            float angleDiff = Mathf.Clamp01(angle / _angleMax); 
            
            drawLines(guide, tool, angleDiff);
        }
    }

    private void drawLines(ScrewGuide guide, TrackedTool tool, float angleDiff)
    {
        _linrendererParent.SetActive(true);

        RaycastHit? hit = tool.HitSpine;
        Vector3 intersection;
        bool intersecting = MedARUtility.LineLineIntersection(out intersection, tool.TooltopPosition, tool.Direction, guide.EntryPosition, guide.Direction);

        Vector3 toolFrom, toolTo, guideFrom, guideTo;
        Vector3 guideMiddle = guide.EntryPosition;

        toolTo = tool.TooltopPosition - _lineMargin * tool.Direction;
        if (intersecting) //Draw from intersection point to tooltop and just as long for screw (+ margin on all ends)
        {
            toolFrom = intersection + _lineMargin * tool.Direction;
            guideFrom = intersection + _lineMargin * guide.Direction;
        }
        else //Draw from tooltop with length = tool length + screw length + margins and same for guide
        {

            guideFrom = guide.EndPosition + _lineMargin * guide.Direction;
            toolFrom = tool.TooltipPosition + (_lineMargin + guide.screwLength) * tool.Direction;
        }
        guideTo = guideFrom - guide.Direction * (Vector3.Distance(toolFrom, toolTo));

        //Draw parts below spine surface semi-transparent - middle = screw position = point of entry of spine
        MedARUtility.DrawLineRendererFromTo(_lineRendererGuideOutsideSpine, guideMiddle, guideTo, _lineWidth);
        MedARUtility.DrawLineRendererFromTo(_lineRendererGuideInsideSpine, guideFrom, guideMiddle, _lineWidth);

        //Draw parts below spine surface semi-transparent
        //Middle = raycasthit point = point of entry tool ray
        if (hit != null)
        {
            Vector3 toolMiddle = hit.Value.point;
            MedARUtility.DrawLineRendererFromTo(_lineRendererToolOutsideSpine, toolMiddle, toolTo, _lineWidth);
            MedARUtility.DrawLineRendererFromTo(_lineRendererToolInsideSpine, toolFrom, toolMiddle, _lineWidth);
        }
        else
        {
            _lineRendererToolInsideSpine.positionCount = 0;
            MedARUtility.DrawLineRendererFromTo(_lineRendererToolOutsideSpine, toolFrom, toolTo, _lineWidth);
        }

        _matLRToolOutsideSpine.color = Color.Lerp(_colorLineToolOutsideSpineCorrect, _colorLineToolOutsideSpineIncorrect, angleDiff);
        _matLRToolInsideSpine.color = Color.Lerp(_colorLineToolInsideSpineCorrect, _colorLineToolInsideSpineIncorrect, angleDiff);
    }

   
}
