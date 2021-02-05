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

    #endregion

    private Phase _currentPhase;



    private void Start()
    {
        startPhases();
    }



    void Update()
    {
        // send current_phase to sonification manager
        SonfificationManager.GetComponent<SonificationManager>().setPhase(_currentPhase);
    }


    public void startPhases()
    {
        _currentPhase = Phase.Position;
        PositionPhase.SetActive(true); // should be set active by default
    }



    public void ActivatePositionPhase()
    {
        OrientationPhase.SetActive(false);
        DepthPhase.SetActive(false);
        PositionPhase.SetActive(true);
        _currentPhase = Phase.Position;
    }

    public void ActivateOrientationPhase()
    {
        PositionPhase.SetActive(false);
        DepthPhase.SetActive(false);
        OrientationPhase.SetActive(true);
        _currentPhase = Phase.Orientation;
    }

    public void ActivateDepthPhase()
    {
        PositionPhase.SetActive(false);
        OrientationPhase.SetActive(false);
        DepthPhase.SetActive(true);
        _currentPhase = Phase.Depth;
    }
}
