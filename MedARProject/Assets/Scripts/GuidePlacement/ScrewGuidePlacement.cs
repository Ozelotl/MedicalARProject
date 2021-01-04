//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// responsible for handling screw behaviour during placement phase
/// </summary>
public class ScrewGuidePlacement : MonoBehaviour, IGazeHandler
{
    [SerializeField]
    private ScrewGuide _guide;

    [SerializeField]
    private GameObject _visualization;
    private Material _visualizationMat;

    //Collider will be transferred from model during creation
    public BoxCollider _collider;
    private bool _interacting; //collider being gazed at (or interacted with tooltip)

    //

    private void OnEnable()
    {
        GazeInputManager.Instance.register(this);
    }
    private void OnDisable()
    {
        GazeInputManager.Instance.deregister(this);
        place(); //so that we don't have dangling screws in visualization phase
    }

    private void Start()
    {
        _visualizationMat = _visualization.GetComponent<Renderer>().material;
    }
    private void Update()
    {
        //set screw guide to be attached to tooltip if it is the one being adjusted
        if (_guide.Focused)
        {
            _guide.transform.position = TrackedTool.Instance.TooltipPosition;
            _guide.transform.rotation = TrackedTool.Instance.Rotation;
        }
    }

    //

    public void place()
    {
        //stop adjusting this guide
        if (_guide.Focused)
            ScrewGuideCollection.Instance.focusedScrewGuide = null;
    }

    //called via local speech command on this gameObject
    public void adjustIfInteracting()
    {
        if (_interacting)
            adjust();
    }
    public void adjust()
    {
        ScrewGuideCollection.Instance.focusedScrewGuide = _guide;
    }

    //called via global speech command on child object (PlacementGlobalSpeechListener)
    public void deleteIfFocused()
    {
        if (_guide.Focused)
            delete();
    }
    //called via local speech command on this gameObject
    public void deleteIfInteracting()
    {
        if (_interacting)
            delete();
    }
    public void delete()
    {
        ScrewGuideCollection.Instance.deleteScrewGuide(_guide);
    }

    //

    //Interaction: if no screw is currently focused, we can adjust and delete other screws
    //Interaction means that no screw is currently focused so basic interaction is enabled
    //After gazing at (or pointing tool at) a screw for x seconds, it will be set to interacting
    //Interacting screw can be deleted/adjusted via speech command

    public void setInteractionEnabled(bool enabled)
    {
        _collider.enabled = enabled;

        if (!enabled)
        {
            if (coroVisualizeInteractable != null)
                StopCoroutine(coroVisualizeInteractable);
        }
    }

    public void OnEnterGaze()
    {
        startSetInteracting(true);
    }
    public void OnExitGaze()
    {
        startSetInteracting(false);
    }
//Can later switch between gaze and tool interaction with screws
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Tooltip"))
    //        startSetInteracting(true);
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Tooltip"))
    //        startSetInteracting(false);
    //}

    private Coroutine coroVisualizeInteractable;
    private void startSetInteracting(bool interacting)
    {
        if (coroVisualizeInteractable != null)
            StopCoroutine(coroVisualizeInteractable);

        coroVisualizeInteractable = StartCoroutine(setInteracting(interacting));
    }
    IEnumerator setInteracting(bool interacting)
    {
        float interval = 0.1f;
        float max = 1.5f;

        for (float f = 0; f < max; f += interval)
        {
            yield return new WaitForSeconds(interval);
            Color c = _visualizationMat.color;
            c.a = f / max;
            if (interacting == false)
                c.a = 1 - c.a;
            _visualizationMat.color = c;
        }

        _interacting = interacting;
        _visualization.SetActive(_interacting);

        coroVisualizeInteractable = null;
    }
}
