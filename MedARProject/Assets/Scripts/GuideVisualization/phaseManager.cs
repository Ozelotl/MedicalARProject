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
public class phaseManager : SingletonMonoMortal<phaseManager>
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
    
    private Phase _currentPhase;
    #endregion


    //private GameObject _currentPhaseObject;
    
    private void Start()
        {
            startPhases();
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
