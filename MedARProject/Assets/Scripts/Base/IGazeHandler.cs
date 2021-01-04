//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface so that a MonoBehaviour implementing this can receive Gaze events from GazeInputManager
/// register/deregister in GazeInputManager Instance in OnEn/Disable 
/// </summary>
public interface IGazeHandler
{
    void OnEnterGaze();
    void OnExitGaze();
}
