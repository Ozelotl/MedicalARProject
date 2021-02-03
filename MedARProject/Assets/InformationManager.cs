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
    


    private Vector3 _currentPosition = Vector3.zero;
    private Quaternion _currentRotation = Quaternion.identity;

    public enum Info
    {
        Show,
        Hide
    }


    #region fields
    [Header("Scene References")]
    [SerializeField]
    private GameObject InformationObject;
    [SerializeField]
    private GameObject OpenButton;
    [SerializeField]
    private GameObject CloseButton;
    [SerializeField]
    private GameObject Content;
    //
    [Header("positioning to camera")]
    [SerializeField]
    private float gazeDistance;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float distanceRight;
    #endregion


    void Start()
    {
        showInfo();
    }

    public void showInfo()
    {
        // make it appear in a fixed relative position to the camera
        _currentPosition = Camera.main.transform.position + gazeDistance*Camera.main.transform.forward + distanceUp * Camera.main.transform.up + distanceRight * Camera.main.transform.right;
        InformationObject.transform.position = _currentPosition;
        InformationObject.transform.rotation = Camera.main.transform.rotation;
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



    private float timer = 0.0f;
    private float waitingTime = 1.0f;


    void Update()
    {
        InformationObject.transform.rotation = Camera.main.transform.rotation;
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            timer = 0f;

        }
    }
}
