using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RegisterSpine : SingletonMonoMortal<RegisterSpine>
{
    [System.Serializable]
    public struct SpineMarker
    {
        public ImageTargetBehaviour imgTarget;
        public Transform transSpine;
        public GameObject overlay;
    }

    [SerializeField]
    private List<SpineMarker> _liMarker;

    private List<Transform> _liTransformsToUse = new List<Transform>();

    [HideInInspector]
    public Vector3 posRegistered;
    [HideInInspector]
    public Quaternion rotRegistered;

    private bool registered;
    private bool Registered
    {
        get { return registered; }
        set
        {
            bool old = registered;
            registered = value;

            if (registered != old)
            {
                if (registered)
                    Spine.Instance.OnMarkerSightGained();
                else
                    Spine.Instance.OnMarkerSightLost();
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < _liMarker.Count; i++)
            _liMarker[i].imgTarget.RegisterOnTrackableStatusChanged(onMarkerStatusChanged);
    }

    private void OnDisable()
    {
        for (int i = 0; i < _liMarker.Count; i++)
            _liMarker[i].imgTarget.UnregisterOnTrackableStatusChanged(onMarkerStatusChanged);
    }

    private void Update()
    {
        if (registered)
        {
            posRegistered = _liTransformsToUse[0].position;
            rotRegistered = _liTransformsToUse[0].rotation;

            for (int i = 1; i < _liTransformsToUse.Count; i++)
            {
                float lerpVal = 1 - (i / ((float)(i + 1)));

                posRegistered = Vector3.Lerp(posRegistered, _liTransformsToUse[i].position, lerpVal);
                rotRegistered = Quaternion.Lerp(rotRegistered, _liTransformsToUse[i].rotation, lerpVal);
            }
        }
    }

    private void onMarkerStatusChanged(TrackableBehaviour.StatusChangeResult res)
    {
        _liTransformsToUse.Clear();
        collectTransforms(TrackableBehaviour.Status.TRACKED, TrackableBehaviour.Status.EXTENDED_TRACKED);
        if (_liTransformsToUse.Count == 0)
            collectTransforms(TrackableBehaviour.Status.DETECTED);
        if (_liTransformsToUse.Count == 0)
            collectTransforms(TrackableBehaviour.Status.LIMITED);

        Registered = _liTransformsToUse.Count != 0;
    }

    private void collectTransforms(TrackableBehaviour.Status status, DataSetTrackableBehaviour.Status? additionalStatus = null)
    {
        for (int i = 0; i < _liMarker.Count; i++)
        {
            SpineMarker m = _liMarker[i];
            if (m.imgTarget.CurrentStatus == status || (additionalStatus != null && m.imgTarget.CurrentStatus == additionalStatus.Value))
                _liTransformsToUse.Add(m.transSpine);
        }
    }

    //

    public void showOverlay()
    {
        setOverlayActive(true);
    }
    public void hideOverlay()
    {
        setOverlayActive(false);
    }
    private void setOverlayActive(bool active)
    {
        for (int i = 0; i < _liMarker.Count; i++)
            _liMarker[i].overlay.SetActive(active);
    }
}
