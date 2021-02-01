//Lars
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// show/hide information on UI
/// </summary>
public class InformationManager : SingletonMonoMortal<InformationManager>
{

    public enum Info
    {
        Show,
        Hide
    }


    #region fields
    [Header("Scene References")]
    [SerializeField]
    private GameObject OpenButton;
    [SerializeField]
    private GameObject CloseButton;
    [SerializeField]
    private GameObject Content;

    #endregion


    private void Start()
    {
        showInfo();
    }

    public void showInfo()
    {
        OpenButton.SetActive(false); // should be set inactive by default
        CloseButton.SetActive(true); // should be set active by default
        Content.SetActive(true); // should be set active by default
    }



    public void hideInfo()
    {
        OpenButton.SetActive(true);
        CloseButton.SetActive(false);
        Content.SetActive(false);
    }

}
