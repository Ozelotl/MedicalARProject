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

    //Used so that a screw can be adjusted/deleted if interacted with by tool
    [SerializeField]
    private InteractableWithTool _interactable;

    //

    private void OnDisable()
    {
        place(); //so that we don't have dangling screws in visualization phase
    }

    private void Update()
    {
        //set screw guide to be attached to tooltip if it is the one being adjusted
        if (_guide.Focused)
        {
            _guide.transform.position = ToolManager.Instance.TooltipPosition;
            _guide.transform.rotation = ToolManager.Instance.Rotation;
        }
    }

    //

    //interactable needs the correct collider size
    public void transferCollider(BoxCollider collider)
    {
        _interactable.transferCollider(collider);
    }

    //

    public void place()
    {
        //stop adjusting this guide
        if (_guide.Focused)
        {
            ScrewGuideCollection.Instance.focusedScrewGuide = null;
            ScrewGuideCollection.Instance.setInteractionEnabled(true);
        }
    }

    //called via local speech command on this gameObject
    public void adjustIfInteracting()
    {
        if (_interactable.Interacting)
            adjust();
    }
    public void adjust()
    {
        ScrewGuideCollection.Instance.focusedScrewGuide = _guide;
        ScrewGuideCollection.Instance.setInteractionEnabled(false);
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
        if (_interactable.Interacting)
            delete();
    }
    public void delete()
    {
        ScrewGuideCollection.Instance.deleteScrewGuide(_guide);
    }

    //

    //Interaction: if no screw is currently focused, we can adjust and delete other screws
    //Interaction means that no screw is currently focused so basic interaction is enabled
    //Interacting screw can be deleted/adjusted via speech command
}
