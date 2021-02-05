//Lars
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// change phases (position, orientation, depth) for different visualization
/// </summary>
public class PhaseManager : SingletonMonoMortal<PhaseManager>
{
    public enum Phase
    {
        Position,
        Orientation,
        Depth
    }

    #region fields
    [Header("Scene References")]
    [SerializeField]
    private GameObject PositionPhase;
    [SerializeField]
    private GameObject OrientationPhase;
    [SerializeField]
    private GameObject DepthPhase;
    [SerializeField]
    private GameObject SonfificationManager;
    [SerializeField]
    private TwoDView _2DView;

    #endregion

    private bool _automatic;
    public bool AutomaticPhase { get { return _automatic; } }
    private Phase _currentPhase;
    public Phase CurrentPhase
    {
        get { return _currentPhase; }
        set
        {
            Phase old = _currentPhase;
            _currentPhase = value;

            if (old != _currentPhase && OnPhaseChanged != null)
                OnPhaseChanged.Invoke(_currentPhase);
        }
    }


    private void Start()
    {
        //Force phase activation at start
        changePhase(Phase.Position, true);
    }
    void Update()
    {
        // send current_phase to sonification manager
        SonfificationManager.GetComponent<SonificationManager>().setPhase(CurrentPhase);
    }
    
    //

    public void ActivateAutomaticPhase()
    {
        _automatic = true;
    }
    public void ActivatePositionPhase()
    {
        changePhase(Phase.Position);
    }
    public void ActivateOrientationPhase()
    {
        changePhase(Phase.Orientation);
    }
    public void ActivateDepthPhase()
    {
        changePhase(Phase.Depth);
    }

    private void changePhase(Phase phase, bool force = false)
    {
        if (!force && phase == _currentPhase)
            return;

        CurrentPhase = phase;

        PositionPhase.SetActive(phase == Phase.Position);
        OrientationPhase.SetActive(phase == Phase.Orientation);
        DepthPhase.SetActive(phase == Phase.Depth);

        _2DView.changePhase(CurrentPhase);
    }

    public delegate void PhaseChangedEvent(Phase phase);
    public PhaseChangedEvent OnPhaseChanged;
}
