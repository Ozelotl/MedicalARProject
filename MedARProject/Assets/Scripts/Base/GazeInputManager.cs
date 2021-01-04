//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;


/// <summary>
/// Manager class so that gameobjects can register for OnGazeEnter and OnGazeExit events
/// For that they need to implement IGazeHandler
/// </summary>
public class GazeInputManager : SingletonMonoMortal<GazeInputManager>
{
    private List<IGazeHandler> _liGazeHandler = new List<IGazeHandler>();
    private IGazeHandler _gazeTarget;

    //call this in OnEnable
    public void register(IGazeHandler gazeHandler)
    {
        if (!_liGazeHandler.Contains(gazeHandler))
            _liGazeHandler.Add(gazeHandler);
    }
    //Call this in OnDisable
    public void deregister(IGazeHandler gazeHandler)
    {
        _liGazeHandler.Remove(gazeHandler);
    }

    private void Update()
    {
        IGazeHandler gazeTargetOld = _gazeTarget;
        _gazeTarget = CoreServices.InputSystem.GazeProvider.GazeTarget ?
            CoreServices.InputSystem.GazeProvider.GazeTarget.GetComponent<IGazeHandler>() : 
            null;

        if (_gazeTarget != gazeTargetOld)
        {
            for (int i = 0; i < _liGazeHandler.Count; i++)
            {
                if (_liGazeHandler[i] == gazeTargetOld)
                    _liGazeHandler[i].OnExitGaze();

                if (_liGazeHandler[i] == _gazeTarget)
                    _liGazeHandler[i].OnEnterGaze();
            }
        }
    }
}
