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
    [SerializeField]
    private GameObject _2DView;

    private void Start()
    {
        ScrewGuideCollection.Instance.enterPhase(ScrewGuideCollection.Phase.Visualization);
    }

    private void Update()
    {
        //TODO: event based
        bool active = _2DView.activeSelf;
        bool activeNew = ScrewGuideCollection.Instance.focusedScrewGuide != null;
        if (activeNew != active)
            _2DView.SetActive(activeNew);
    }
}
