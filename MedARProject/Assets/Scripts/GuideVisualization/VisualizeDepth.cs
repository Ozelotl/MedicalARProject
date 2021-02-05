//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// In Visualize Depth Phase:
///     Show ruler overlain and distance in spine and remaining as nrs next to ruler
///     Change color when too deep as warning
/// </summary>
public class VisualizeDepth : MonoBehaviour
{
    public bool test = true;

    [SerializeField]
    private GameObject _visObjectsParent;

    [SerializeField]
    private Transform _torus;
    [SerializeField]
    private LineRenderer _lrOutsideSpine;
    [SerializeField]
    private LineRenderer _lrInsideSpine;

    [SerializeField]
    private Transform _transTextToolDepth;
    [SerializeField]
    private TMPro.TMP_Text _textToolDepth;
    [SerializeField]
    private Transform _transTextRemDepth;
    [SerializeField]
    private TMPro.TMP_Text _textRemainingDepth;

    [Header("Settings")]
    [SerializeField]
    private float _lineWidth;
    [SerializeField]
    private float _lineSegmentLength;
    [SerializeField]
    private float _alphaInside;

    [SerializeField]
    private Color _colTextOK;
    [SerializeField]
    private Color _colTextWarning;

    [SerializeField]
    private GameObject SonfificationManager;

    private Camera cam;

    private void Start()
    {
        _lrOutsideSpine.widthMultiplier = _lineWidth;
        _lrInsideSpine.widthMultiplier = _lineWidth;

        Renderer rendererInsideSpine = _lrInsideSpine.GetComponent<Renderer>();
        Color c = rendererInsideSpine.material.color;
        c.a = _alphaInside;
        rendererInsideSpine.material.color = c;

        cam = Camera.main; //find the mrtk camera
    }

    private void Update()
    {
        ScrewGuide guide = ScrewGuideCollection.Instance.focusedScrewGuide;
        if (guide == null)
        {
            _visObjectsParent.SetActive(false);
            SonfificationManager.GetComponent<SonificationManager>().setDrillingInformation(-1, -1); //negative distance values indicate that no screw is in focus --> no sonification
        }
        else
        {
            _visObjectsParent.SetActive(true);
         
            TrackedTool tool = TrackedTool.Instance;

            //Get position on tool that would be just visible if tool was fully inside spine for the whole screwLength
            Vector3 toolFullyInsidePos = tool.TooltipPosition - tool.Direction * guide.screwLength;
            //Set indicator to that position
            _torus.transform.position = toolFullyInsidePos;
            _torus.transform.rotation = tool.Rotation;

            RaycastHit? hit = tool.HitSpine;

            float depthTool = 0f;
            float remainingDepth = guide.screwLength;

            if (hit == null)
            {
                //If we are currently not holding the tool in a position that makes sense, just show a ruler over the screwguide
                _lrOutsideSpine.positionCount = 0;
                MedARUtility.DrawLineRendererFromTo(_lrInsideSpine, guide.EntryPosition, guide.EndPosition, _lineSegmentLength);

                remainingDepth = guide.screwLength;
            }
            if (hit != null)
            {
                //If we hold the tool to the screwguide: calculate distance that tool is in spine
                depthTool = tool.Length - Vector3.Distance(hit.Value.point, tool.TooltopPosition);
                if (depthTool < 0)
                    depthTool = 0f;

                //From that calculate how far the tool still needs to go inside the spine
                remainingDepth = guide.screwLength - depthTool;

                //Draw rulers from guide end to toolFullyInsidePos
                MedARUtility.DrawLineRendererFromTo(_lrOutsideSpine, hit.Value.point, toolFullyInsidePos, _lineSegmentLength);
                MedARUtility.DrawLineRendererFromTo(_lrInsideSpine, hit.Value.point, tool.Direction, guide.screwLength, _lineSegmentLength);
            }

            _textToolDepth.gameObject.SetActive(Mathf.Abs(depthTool) > 0.0001f);
            bool ok = remainingDepth >= 0;

            //Show tool depth and remaining depth as numbers
            //They are red if we are too deep
            _transTextToolDepth.position = guide.EntryPosition + (tool.Direction * 0.5f * depthTool);
            _transTextToolDepth.LookAt(_transTextToolDepth.transform.position + cam.transform.forward.normalized, Vector3.up);
            _textToolDepth.text = MedARUtility.unitsToText(depthTool, 1000);
            _textToolDepth.color = ok ? _colTextOK : _colTextWarning;

            _transTextRemDepth.position = guide.EndPosition - (tool.Direction*0.5f*remainingDepth);
            _transTextRemDepth.LookAt(_textRemainingDepth.transform.position + cam.transform.forward.normalized, Vector3.up);
            _textRemainingDepth.text = MedARUtility.unitsToText(remainingDepth, 1000);
            _textRemainingDepth.color = ok ? _colTextOK : _colTextWarning;

            SonfificationManager.GetComponent<SonificationManager>().setDrillingInformation(guide.screwLength, depthTool); // send information regarding the drilling to sonification manager
        }

        
    }

}
