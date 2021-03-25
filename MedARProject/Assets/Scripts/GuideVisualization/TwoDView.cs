//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// for each phase, a window with 2D views opens itself
/// these are rendered by additional cameras
/// content is activated upon gaze
/// </summary>
public class TwoDView : MonoBehaviour, IGazeHandler
{
    [System.Serializable]
    public class View
    {
        [SerializeField]
        private GameObject _view;
        private GameObject _content;
        private BoxCollider _collider;
        private Material _matBG;

        public void init()
        {
            Transform bg = _view.transform.GetChild(0);
            _content = bg.GetChild(0).gameObject;
            _collider = _view.GetComponent<BoxCollider>();
            _collider.enabled = false;
            _matBG = bg.GetComponent<Renderer>().material;
        }

        public void setActive(bool active)
        {
            _view.SetActive(active);
        }
        public void setContentActive(bool active)
        {
            _content.SetActive(active);
        }

        public BoxCollider collider { get { return _collider; } }

        public void fadeBG(float a)
        {
            Color c = _matBG.color;
            c.a = a;
            _matBG.color = c;
        }
        public float Alpha { get { return _matBG.color.a; } }
    }

    #region Settings
    [Header("Settings")]
    [SerializeField]
    private float _alphaMin;
    [SerializeField]
    private float _alphaMax;
    [SerializeField]
    private float _fadeDuration;
    [SerializeField]
    private float _fadeInterval;

    [SerializeField]
    private View _position;
    [SerializeField]
    private View _orientation;
    [SerializeField]
    private View _depth;
    #endregion

    private BoxCollider _colThis;
    private Vector3 _colSize;

    private bool unpinned = false;//has the window been moved manually
    private Vector3 _posLast;
    private Quaternion _rotLast;

    private Camera _cam;

    private bool _guideFocused = false;//is any screw guide currently active
    private bool _lookedAt = false; //is the window gazed at

    //Gaze - fade content in when the view is gazed at 

    void OnEnable()
    {
        GazeInputManager.Instance.register(this);
    }

    void OnDisable()
    {
        GazeInputManager.Instance.deregister(this);
    }

    public void OnEnterGaze()
    {
        startSetLookAt(true);
    }

    public void OnExitGaze()
    {
        startSetLookAt(false);
    }

    //

    void Awake()
    {
        init();
    }

    private bool _initialized = false;
    //lazy because PhaseManager starts before our awake because of execution order
    private void init()
    {
        if (_initialized)
            return;
        _initialized = true;

        _cam = Camera.main;

        _position.init();
        _orientation.init();
        _depth.init();

        _colThis = gameObject.GetComponent<BoxCollider>();
        _colSize = _colThis.size;

        _lookedAt = false;
        enableContent(false);
        _position.fadeBG(_alphaMin);
        _orientation.fadeBG(_alphaMin);
        _depth.fadeBG(_alphaMin);

        _posLast = transform.position;
        _rotLast = transform.rotation;
    }

    void Update()
    {
        //There is a bug where collider size doesn't get set correctly at start...
        if (_colThis.size != _colSize)
            _colThis.size = _colSize;

        bool guideFocusedNew = ScrewGuideCollection.Instance.focusedScrewGuide != null;
        if (guideFocusedNew != _guideFocused)
        {
            _guideFocused = guideFocusedNew;
            enableContent(_lookedAt && _guideFocused);
        }

        init();

        if (unpinned)
            return;

        if (transform.position != _posLast || transform.rotation != _rotLast)
            unpinned = true;

        if (!unpinned) //if the window is still pinned to a screw guide, update its position when the spine moves
            setPinnedTransform();

        _posLast = transform.position;
        _rotLast = transform.rotation;
    }

    private void setPinnedTransform()//Per default, the 2D view is positioned relative to the current screw guide
    {
        //One must be focused otherwise we are inactive;
        ScrewGuide guide = ScrewGuideCollection.Instance.focusedScrewGuide;
        if (guide == null)
            return;

        transform.position = guide.EntryPosition - _cam.transform.right * 0.075f + _cam.transform.forward * 0.075f  + Vector3.up * 0.1f;
        transform.LookAt(transform.position - _cam.transform.forward.normalized, Vector3.up);
    }

    public void changePhase(PhaseManager.Phase phase)//change active content based on active phase
    {
        init();

        _position.setActive(phase == PhaseManager.Phase.Position);
        _orientation.setActive(phase == PhaseManager.Phase.Orientation);
        _depth.setActive(phase == PhaseManager.Phase.Depth);

        BoxCollider colToUse = _colThis;

        switch (phase)
        {
            case PhaseManager.Phase.Position:
                colToUse = _position.collider;
                break;
            case PhaseManager.Phase.Orientation:
                colToUse = _orientation.collider;
                break;
            case PhaseManager.Phase.Depth:
                colToUse = _depth.collider;
                break;
        }

        _colThis.center = colToUse.center;
        _colThis.size = colToUse.size;
        _colSize = colToUse.size;
    }

    // Activate/deactivate based on gaze

    //After gazing at the view for x seconds, it will be set to interacting
    private Coroutine coroLookAt;
    private void startSetLookAt(bool lookAt)
    {
        if (coroLookAt != null)
            StopCoroutine(coroLookAt);

        coroLookAt = StartCoroutine(setLookAt(lookAt));
    }
    IEnumerator setLookAt(bool lookAt)
    {
        float interval = 0.1f;

        float alphaStart = _position.Alpha;
        float alphaTarget = lookAt ? _alphaMax : _alphaMin;

        float dur = Mathf.Abs(alphaStart - alphaTarget) / (_alphaMax - _alphaMin) * _fadeDuration;
        float alphaStep = interval / dur * (alphaTarget - alphaStart);

        for (float f = 0; f < dur; f += interval)
        {
            yield return new WaitForSeconds(interval);

            float alphaCur = _position.Alpha;
            float alpha = alphaCur + alphaStep;

            _position.fadeBG(alpha);
            _orientation.fadeBG(alpha);
            _depth.fadeBG(alpha);
        }
        _position.fadeBG(alphaTarget);
        _orientation.fadeBG(alphaTarget);
        _depth.fadeBG(alphaTarget);

        _lookedAt = lookAt;
        enableContent(_lookedAt && _guideFocused);

        coroLookAt = null;
    }

    private void enableContent(bool enabled)
    {
        _position.setContentActive(enabled);
        _orientation.setContentActive(enabled);
        _depth.setContentActive(enabled);
    }
}
