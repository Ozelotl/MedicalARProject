//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In Visualize Position Phase:
///     Show crosshair projection of Tooltip onto Spine with Spine Normal, do not block by geometry
///     Change color when touching spine as feedback for virtual model
///     When near a ScrewGuide (interacting), additionally show lines with distance numbers and ruler
/// </summary>
public class VisualizePosition : MonoBehaviour
{
    [SerializeField]
    private Color _colorCrosshairProjection;
    [SerializeField]
    private Color _colorCrosshairOnSpine;
    [SerializeField]
    private Color _colorIndicatorProjection;
    [SerializeField]
    private Color _colorIndicatorOnSpine;

    //sprites

    [SerializeField]
    private SpriteRenderer _crosshair;
    [SerializeField]
    private SpriteRenderer _indicator;

    [SerializeField]
    private TMPro.TMP_Text _indicatorTextX;
    [SerializeField]
    private TMPro.TMP_Text _indicatorTextY;


    private void Update()
    {
        if (TrackedTool.Instance.CurrentState == TrackedTool.ToolState.InSpine)
            _crosshair.gameObject.SetActive(false);
        else
            visualizeCrosshair();
    }

    private void visualizeCrosshair()
    {
        if (TrackedTool.Instance.HitSpine == null)
            _crosshair.gameObject.SetActive(false);
        else
        {
            _crosshair.gameObject.SetActive(true);

            //Change color when touching spine as feedback for virtual model
            _crosshair.color = TrackedTool.Instance.CurrentState == TrackedTool.ToolState.OnSpine ? _colorCrosshairOnSpine : _colorCrosshairProjection;
            _indicator.color = TrackedTool.Instance.CurrentState == TrackedTool.ToolState.OnSpine ? _colorIndicatorOnSpine : _colorIndicatorProjection; ;

            RaycastHit hit = TrackedTool.Instance.HitSpine.Value;
            ScrewGuide guideFocused = ScrewGuideCollection.Instance.focusedScrewGuide;

            //It's better to lock rotation to screw guide rotation when near
            Vector3 normal = (guideFocused == null ? hit.normal : -guideFocused.Direction);

            //Project onto Spine with spine normal
            _crosshair.transform.position = hit.point;
            _crosshair.transform.LookAt(hit.point - normal.normalized);
            //Calculate X and Y so they are analogous to Spine x and y
            Vector3 crosshairUp = Vector3.ProjectOnPlane(Spine.Instance.transform.forward, normal);
            float angle = Vector3.SignedAngle(_crosshair.transform.up, crosshairUp, normal);
            _crosshair.transform.Rotate(normal, angle, Space.World);

            if (guideFocused == null)
            {
                _indicator.gameObject.SetActive(false);
                _indicatorTextX.gameObject.SetActive(false);
                _indicatorTextY.gameObject.SetActive(false);
            }
            else
            {
                //Show ruler
                _indicator.gameObject.SetActive(true);
                _indicator.transform.position = guideFocused.visualization.PosGuide;
                _indicatorTextX.gameObject.SetActive(true);
                _indicatorTextY.gameObject.SetActive(true);

                Bounds boundsCrosshair = _crosshair.GetComponent<SpriteRenderer>().bounds;

                Vector2 dist = new Vector2(_indicator.transform.localPosition.x, _indicator.transform.localPosition.y);

                //Set ruler position
                _indicator.transform.position.Set
                (
                    Mathf.Clamp(_indicator.transform.position.x, boundsCrosshair.min.x, boundsCrosshair.max.x),
                    Mathf.Clamp(_indicator.transform.position.y, boundsCrosshair.min.y, boundsCrosshair.max.y),
                    _indicator.transform.position.z
                );
                _indicator.transform.localPosition.Set
                (
                    _indicator.transform.localPosition.x, 
                    _indicator.transform.localPosition.y,
                    0f
                );

                //show distance as text
                _indicatorTextX.text = unitsToText(dist.x);
                _indicatorTextY.text = unitsToText(dist.y);
                _indicatorTextX.transform.position.Set
                (
                    _indicatorTextX.transform.position.x,
                    Mathf.Abs(_indicatorTextX.transform.position.y * Mathf.Sign(dist.y)),
                    _indicatorTextX.transform.position.z
                );
                _indicatorTextX.transform.position.Set
                (
                    Mathf.Abs(_indicatorTextX.transform.position.x * Mathf.Sign(dist.x)),
                    _indicatorTextX.transform.position.y,
                    _indicatorTextX.transform.position.z
                );
            }
        }
    }

    private string unitsToText(float units)
    {
        //1 Unity unit = 5mm

        float mm = units * 5;
        if (Mathf.Abs(mm) > 10)
            return (mm / 10.0f).ToString("0.0") + " cm";
        else
            return mm.ToString("0.0") + " mm";
    }
}
