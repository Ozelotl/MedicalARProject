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
        bool intersecting = LineLineIntersection(out intersection, tool.TooltopPosition, tool.Direction, guide.EntryPosition, guide.Direction);

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
        drawLineRendererFromTo(_lineRendererGuideOutsideSpine, guideMiddle, guideTo);
        drawLineRendererFromTo(_lineRendererGuideInsideSpine, guideFrom, guideMiddle);

        //Draw parts below spine surface semi-transparent
        //Middle = raycasthit point = point of entry tool ray
        if (hit != null)
        {
            Vector3 toolMiddle = hit.Value.point;
            drawLineRendererFromTo(_lineRendererToolOutsideSpine, toolMiddle, toolTo);
            drawLineRendererFromTo(_lineRendererToolInsideSpine, toolFrom, toolMiddle);
        }
        else
        {
            _lineRendererToolInsideSpine.positionCount = 0;
            drawLineRendererFromTo(_lineRendererToolOutsideSpine, toolFrom, toolTo);
        }

        _matLRToolOutsideSpine.color = Color.Lerp(_colorLineToolOutsideSpineCorrect, _colorLineToolOutsideSpineIncorrect, angleDiff);
        _matLRToolInsideSpine.color = Color.Lerp(_colorLineToolInsideSpineCorrect, _colorLineToolInsideSpineIncorrect, angleDiff);
    }

    private void drawLineRendererFromTo(LineRenderer lr, Vector3 from, Vector3 to)
    {
        drawLineRendererFromTo(lr, from, (to - from).normalized, Vector3.Distance(from, to));
    }

    private void drawLineRendererFromTo(LineRenderer lr, Vector3 start, Vector3 dir, float dist)
    {
        List<Vector3> liPos = new List<Vector3>();

        Vector3 dirN = dir.normalized;
        float distCur = 0f;

        //set segments with length equal to width to keep texture from being squashed
        while (distCur < dist)
        {
            int count = liPos.Count;
            liPos.Add(start + count * _lineWidth * dirN);
            distCur += _lineWidth;
        }
        liPos.Add(start + liPos.Count * _lineWidth * dirN);

        lr.positionCount = liPos.Count;
        lr.SetPositions(liPos.ToArray());
    }

    //Intersection between 2 3D Line segments (From Unity Community Wiki)
    private static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Approximately(planarFactor, 0f) &&
            !Mathf.Approximately(crossVec1and2.sqrMagnitude, 0f))
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }
}
