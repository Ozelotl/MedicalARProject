//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// responsible for basic game states and scene loading
/// </summary>
public class StateManager : SingletonMonoMortal<StateManager>
{
    public enum ApplicationState
    { 
        Intro, // = base scene and intro scene, ends after timer or on next
        Menu, // = base scene + screw guide scene + menu scene
        GuidePlacement, // = base scene + screw guide scene + placement scene
        GuideVisualization // = base scene + screw guide scene + visualization scene, going next step from here loops around to menu
    }

    #region fields
    [Header("Scene References")]
    [SerializeField]
    private string nameIntroScene;
    [SerializeField]
    private string nameScrewGuidesScene;
    [SerializeField]
    private string nameMenuScene;
    [SerializeField]
    public string nameGuidePlacementScene;
    [SerializeField]
    public string nameGuideVisualizationScene;

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

        int nextState = ++curState;
        if (nextState == statesCount)
            nextState = (int)ApplicationState.Menu;

        setState((ApplicationState)nextState);
    }
    public void enterPreviousState()
    {
        int curState = (int)_state;
        int statesCount = System.Enum.GetValues(typeof(ApplicationState)).Length;
        int prevState = --curState;

        if (prevState <= 0)
            prevState = (int)ApplicationState.Menu;

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
                SceneManager.LoadScene(nameScrewGuidesScene, LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync(nameIntroScene);
                break;
            case ApplicationState.Menu:
                SceneManager.UnloadSceneAsync(nameMenuScene);
                break;
            case ApplicationState.GuidePlacement:
                SceneManager.UnloadSceneAsync(nameGuidePlacementScene);
                break;
            case ApplicationState.GuideVisualization:
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
            case ApplicationState.Intro: //only happens once at start so we don't need to unload anything
                SceneManager.LoadScene(nameIntroScene, LoadSceneMode.Additive);
                break;
            case ApplicationState.Menu:
                SceneManager.LoadScene(nameMenuScene, LoadSceneMode.Additive);
                break;
            case ApplicationState.GuidePlacement:
                SceneManager.LoadScene(nameGuidePlacementScene, LoadSceneMode.Additive);
                break;
            case ApplicationState.GuideVisualization:
                SceneManager.LoadScene(nameGuideVisualizationScene, LoadSceneMode.Additive);
                break;
            default:
                break;
        }

        _state = newState;
    }
    #endregion
}
