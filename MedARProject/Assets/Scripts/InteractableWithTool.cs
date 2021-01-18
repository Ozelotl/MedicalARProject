using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableWithTool : MonoBehaviour, IGazeHandler
{
    [SerializeField]
    private GameObject _visualization;
    private Material _visualizationMat;

    public BoxCollider colliderInteraction;
    public Vector3 colliderMargin;
    private bool _interacting; //collider being interacted with tooltip
    public bool Interacting
    {
        get { return _interacting; }
    }

    //
    private void Start()
    {
        _visualizationMat = _visualization.GetComponent<Renderer>().material;
        setInteractionEnabled(false);
    }

    private void OnEnable()
    {
        GazeInputManager.Instance.register(this);
        ScrewGuideCollection.Instance.OnInteractionEnabled += setInteractionEnabled;
    }
    private void OnDisable()
    {
        GazeInputManager.Instance.deregister(this);
        ScrewGuideCollection.Instance.OnInteractionEnabled -= setInteractionEnabled;
    }

    //

    public void transferCollider(BoxCollider collider)
    {
        colliderInteraction.center = collider.center;
        colliderInteraction.size = collider.size + colliderMargin;
    }

    //

    public void setInteractionEnabled(bool enabled)
    {
        colliderInteraction.enabled = enabled;

        if (coroVisualizeInteractable != null)
            StopCoroutine(coroVisualizeInteractable);
        coroVisualizeInteractable = null;

        _interacting = false;

        _visualization.SetActive(false);
    }

    //Adjust/delete screw tool is pointing at
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tooltip"))
            startSetInteracting(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tooltip"))
            startSetInteracting(false);
    }

    //Can we later enable gaze as well? what would the priority be? switch modes?
    public void OnEnterGaze()
    {
        //startSetInteracting(true);
    }
    public void OnExitGaze()
    {
        //startSetInteracting(false);
    }

    //After gazing at (or pointing tool at) a screw for x seconds, it will be set to interacting
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
