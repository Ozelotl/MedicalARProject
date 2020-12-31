//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// responsible for handling screw behaviour during visualization phase
/// </summary>
public class ScrewGuideVisualization : MonoBehaviour
{
    [SerializeField]
    private ScrewGuide _guide;
    private Transform _guideTransform;

    [SerializeField]
    private GameObject _showFocused;

    private bool focused;

    private void Awake()
    {
        _guideTransform = _guide.transform;
    }

    private void Update()
    {
        if (focused != _guide.Focused)
        {
            focused = _guide.Focused;
            showVisualizationFocused();
        }

        //Dummy visualization: log values and show line gizmos in scene
        if (focused)
        {
            Vector3 posGuide = _guideTransform.position;
            Vector3 posTool = TrackedTool.Instance.TooltipPosition;
            float distPos = Vector3.Distance(posGuide, posTool);

            Vector3 dirGuide = _guide.Direction;
            Vector3 dirTool = TrackedTool.Instance.Direction;
            float angle = Mathf.Abs(Vector3.Angle(dirGuide, dirTool));

            bool ok = distPos < 0.01f && angle < 5f;

            Debug.Log
            (
                (ok ? "OK" : "Not OK") + ": \n" +
                "Screw Guide position: " + posGuide.ToString("F4") + " - Tooltip position: " + posTool.ToString("F4") + " - Distance: " + distPos.ToString("F4") + "\n" +
                "Screw Guide direction: " + dirGuide.ToString("F4") + " - Tool direction: " + dirTool.ToString("F4") + " - Angle: " + angle.ToString("F2")
            );

            Debug.DrawLine(_guideTransform.position, _guideTransform.position - _guide.Direction * 0.1f, Color.blue, 0, false);
            Debug.DrawLine(TrackedTool.Instance.TooltipPosition, _guideTransform.position + TrackedTool.Instance.Direction * 0.1f, ok ? Color.green: Color.red, 0, false);
        }
    }

    private void showVisualizationFocused()
    {
        //enables a red ring gameObject to show which screw is being visualized at this moment
        _showFocused.SetActive(focused);
    }
}
