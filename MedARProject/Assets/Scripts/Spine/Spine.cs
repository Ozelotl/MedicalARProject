//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Responsible for Spine positioning behaviour modes
/// Switch between registered and non-registered
/// Non-registered uses nearInteractionGrabbable + bounding box for adjustments
/// 
/// Currently: 
/// if non-registered and markers are in sight for more than x seconds -> registered
/// if registered and markers are out of sight for more than x seconds -> non-registered
/// 
/// (Test using B key: down = marker sight, up = no marker sight)
/// </summary>
public class Spine : SingletonMonoMortal<Spine>
{
    [SerializeField]
    private Transform _model;
    [SerializeField]
    private Transform _collider;

    [SerializeField]
    private GameObject _nonRegisteredLogicObject;
    [SerializeField]
    private Transform _nonRegisteredLogicTarget;

    [SerializeField]
    private GameObject _loadingBarObject;
    [SerializeField]
    private ProgressIndicatorLoadingBar _loadingBar;


    private bool _registered;

    private void Update()
    {
        //Since there are two ways to position the spine, we move logic objects
        //Only the logic object for the current phase is active
        //Because of that, we need to copy transform values from the respective logic objects

        if (!_registered)
        {
            setTransform(_nonRegisteredLogicTarget);
        }
        else
        { 
            //TODO: set transform to registered values
        }

        //DUMMY for testing
        if (Input.GetKeyDown(KeyCode.B))
            OnMarkerSightGained();
        if (Input.GetKeyUp(KeyCode.B))
            OnMarkerSightLost();
    }

    private void setTransform(Transform t)
    {
        _model.transform.position = t.position;
        _model.transform.rotation = t.rotation;
        _model.transform.localScale = t.localScale;
        _collider.transform.position = t.position;
        _collider.transform.rotation = t.rotation;
        _collider.transform.localScale = t.localScale;
    }

    private void OnMarkerSightGained()
    {
        Debug.Log("Gained sight of Spine markers!");
        stopSwitchPhase();
        if (!_registered)
            _coroSwitchPhase = StartCoroutine(switchPhase(true));
    }
    private void OnMarkerSightLost()
    {
        Debug.Log("Lost sight of Spine markers!");
        stopSwitchPhase();
        if (_registered)
            _coroSwitchPhase = StartCoroutine(switchPhase(false));
    }

    private Coroutine _coroSwitchPhase;
    private System.Threading.Tasks.Task _taskCloseLoadingBar;

    private IEnumerator switchPhase(bool registered)
    {
        //Wait until loading bar has closed if closed by cancellation of switch
        while (_taskCloseLoadingBar != null && !_taskCloseLoadingBar.IsCompleted)
            yield return null;
        _taskCloseLoadingBar = null;

        //reset and open loading bar
        _loadingBar.Progress = 0f;
        System.Threading.Tasks.Task taskOpen = _loadingBar.OpenAsync();
        while (!taskOpen.IsCompleted)
            yield return null;
        _loadingBarObject.SetActive(true);

        _loadingBar.Message = registered ? "register..." : "marker sight lost...";

        //Update Progress
        float interval = 0.1f;
        float max = 2f;
        for (float f = 0; f < max; f += interval)
        {
            yield return new WaitForSeconds(interval);
            _loadingBar.Progress = f / max;
        }

        //Close Loading Bar
        System.Threading.Tasks.Task taskClose = _loadingBar.CloseAsync();
        while (!taskClose.IsCompleted)
            yield return null;
        _loadingBarObject.SetActive(false);
        _registered = registered;

        if (!_registered)
        {
            //switching to manual positioning - update bounding box to be at last transform values
            _nonRegisteredLogicTarget.transform.position = _model.transform.position;
            _nonRegisteredLogicTarget.transform.rotation = _model.transform.rotation;
            _nonRegisteredLogicTarget.transform.localScale = _model.transform.localScale;
        }

        //Set respective logic objects active/inactive
        _nonRegisteredLogicObject.SetActive(!_registered);
    }

    private void stopSwitchPhase()
    {
        //Stops switching phase process if sight gained/lost again during switching process
        if (_coroSwitchPhase != null)
            StopCoroutine(_coroSwitchPhase);

        _taskCloseLoadingBar = _loadingBar.CloseAsync();
        _loadingBarObject.SetActive(false);
    }
}


