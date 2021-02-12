using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class RegisterSpine : SingletonMonoMortal<RegisterSpine>
{
    [System.Serializable]
    public class SpineMarker
    {
        public ImageTargetBehaviour imgTarget;
        public GameObject goOverlay;
        public Transform transSpine;
        public Transform transStyrofoam;
    }

    public List<SpineMarker> liMarker;

    private Transform _lastTracked;
    private Transform[] _arTransToUse;
    private int _iTransToUse;

    [SerializeField]
    private AdjustSpinePos _adjust;

    [HideInInspector]
    public Vector3 posRegistered;
    [HideInInspector]
    public Quaternion rotRegistered;

    private bool registered;
    public bool Registered
    {
        get { return registered; }
        set
        {
            bool old = registered;
            registered = value;

            if (registered != old && Spine.Instance != null)
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
        for (int i = 0; i < liMarker.Count; i++)
            liMarker[i].imgTarget.RegisterOnTrackableStatusChanged(onMarkerStatusChanged);
    }

    private void OnDisable()
    {
        for (int i = 0; i < liMarker.Count; i++)
            liMarker[i].imgTarget.UnregisterOnTrackableStatusChanged(onMarkerStatusChanged);
    }

    private void Start()
    {
        _arTransToUse = new Transform[liMarker.Count];
    }

    private void Update()
    {
        if (registered)
        {
            Vector3 posRegisteredNew = _arTransToUse[0].position;
            Quaternion rotRegisteredNew = _arTransToUse[0].rotation;

            for (int i = 1; i < _iTransToUse; i++)
            {
                float lerpVal = 1 - (i / ((float)(i + 1)));

                posRegisteredNew = Vector3.Lerp(posRegisteredNew, _arTransToUse[i].position, lerpVal);
                rotRegisteredNew = Quaternion.Lerp(rotRegisteredNew, _arTransToUse[i].rotation, lerpVal);
            }

            if (transEqualLossy(posRegistered, rotRegistered.eulerAngles, posRegisteredNew, rotRegisteredNew.eulerAngles, 0.05f, 0.005f))
            {
                posRegistered = Vector3.Lerp(posRegistered, posRegisteredNew, 0.7f);
                rotRegistered = Quaternion.Lerp(rotRegistered, rotRegisteredNew, 0.7f);
            }
            else
            {
                posRegistered = posRegisteredNew;
                rotRegistered = rotRegisteredNew;
            }
        }
    }

    private void onMarkerStatusChanged(TrackableBehaviour.StatusChangeResult res)
    {
        _iTransToUse = 0;
        bool registerNew = false;

        collectTransforms(TrackableBehaviour.Status.TRACKED);

        if (_iTransToUse > 0)
        {
            _lastTracked = _arTransToUse[0];
            registerNew = true;
        }

        collectTransforms(TrackableBehaviour.Status.EXTENDED_TRACKED, _lastTracked);


        if (!Registered && registerNew)
            Registered = true;
        else if (Registered && _iTransToUse == 0)
            Registered = false;
    }

    private void collectTransforms(TrackableBehaviour.Status status, Transform lastTracked = null)
    {
        for (int i = 0; i < liMarker.Count; i++)
        {
            SpineMarker m = liMarker[i];
            if (m.imgTarget.CurrentStatus == status && (lastTracked == null || transEqualLossy(m.transSpine, lastTracked, 0.02f)))
                _arTransToUse[_iTransToUse++] = (m.transSpine);
        }
    }

    private static bool transEqualLossy(Transform t1, Transform t2, float lossy, float lossyMin = -1)
    {
        return transEqualLossy(t1.position, t1.eulerAngles, t2.position, t2.eulerAngles, lossy, lossyMin);
    }

    private static bool transEqualLossy(Vector3 pos1, Vector3 eAng1, Vector3 pos2, Vector3 eAng2, float lossy, float lossyMin = -1)
    {
        float distPos = Vector3.Distance(pos1, pos2);
        float distRot = Vector3.Distance(eAng1, eAng2);

        bool eq = distPos < lossy && distRot < lossy;
        if (lossyMin >= 0 && (distPos < lossyMin || distRot < lossyMin))
            eq = false;
        return eq;
    }

    //

    //Called via voice command
    public void startAdjust()
    {
        if (StateManager.Instance.CurrentState == StateManager.ApplicationState.Adjust)
            _adjust.startAdjust();
    }

    public void showOverlay()
    {
        for (int i = 0; i < liMarker.Count; i++)
            liMarker[i].goOverlay.SetActive(true);
    }

    public void hideOverlay()
    {
        for (int i = 0; i < liMarker.Count; i++)
            liMarker[i].goOverlay.SetActive(false);
    }

    public void setVirtual()
    {
        Registered = false;
    }
}
