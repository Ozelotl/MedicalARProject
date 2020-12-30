//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class StateManager : SingletonMonoImmortal
{
    public enum ApplicationState
    { 
        Intro, 
        GuidePlacement, 
        GuideVisualization
    }

    #region fields
    [Header("Scene References")]
    public string nameBaseScene;
    public string nameIntroScene;
    public string nameGuidePlacementScene;
    public string nameGuideVisualizationScene;

    [Header("BaseScene events on state switch")]
    public UnityEvent OnEnterIntroScene;
    public UnityEvent OnEnterGuidePlacementScene;
    public UnityEvent OnEnterGuideVisualizationScenee;
    [Space]
    public UnityEvent OnExitIntroScene;
    public UnityEvent OnExitGuidePlacementScene;
    public UnityEvent OnExitGuideVisualizationScene;

    private ApplicationState _state;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        enterState(ApplicationState.Intro);
    }

    #region API
    public void enterNextState()
    {
        int curState = (int)_state;
        int statesCount = System.Enum.GetValues(typeof(ApplicationState)).Length;
        int nextState = ++curState % statesCount;
        setState((ApplicationState)nextState);
    }
    public void enterPreviousState()
    {
        int curState = (int)_state;
        int statesCount = System.Enum.GetValues(typeof(ApplicationState)).Length;
        int prevState = (--curState + statesCount) % statesCount;
        setState((ApplicationState)prevState);
    }
    public void setState(ApplicationState stateNew)
    {
        exitState(_state);
        enterState(stateNew);
    }
    #endregion

    #region internal State Machine
    private void exitState(ApplicationState oldState)
    {
        Debug.Log("Exit State " + oldState);

        switch (oldState)
        {
            case ApplicationState.Intro:
                if (OnExitIntroScene != null)
                    OnExitIntroScene.Invoke();
                SceneManager.UnloadSceneAsync(nameIntroScene);
                break;
            case ApplicationState.GuidePlacement:
                if (OnExitGuidePlacementScene != null)
                    OnExitGuidePlacementScene.Invoke();
                SceneManager.UnloadSceneAsync(nameGuidePlacementScene);
                break;
            case ApplicationState.GuideVisualization:
                if (OnExitGuideVisualizationScene != null)
                    OnExitGuideVisualizationScene.Invoke();
                SceneManager.UnloadSceneAsync(nameGuideVisualizationScene);
                break;
            default:
                break;
        }
    }
    private void enterState(ApplicationState newState)
    {
        Debug.Log("Enter State " + newState);

        switch (newState)
        {
            case ApplicationState.Intro:
                if (OnEnterIntroScene != null)
                    OnEnterIntroScene.Invoke();
                SceneManager.LoadScene(nameIntroScene, LoadSceneMode.Additive);
                break;
            case ApplicationState.GuidePlacement:
                if (OnEnterGuidePlacementScene != null)
                    OnEnterGuidePlacementScene.Invoke();
                SceneManager.LoadScene(nameGuidePlacementScene, LoadSceneMode.Additive);
                break;
            case ApplicationState.GuideVisualization:
                if (OnEnterGuideVisualizationScenee != null)
                    OnEnterGuideVisualizationScenee.Invoke();
                SceneManager.LoadScene(nameGuideVisualizationScene, LoadSceneMode.Additive);
                break;
            default:
                break;
        }

        _state = newState;
    }
    #endregion
}
