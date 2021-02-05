//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera automatically gets set to 0.3 clip plane
/// but we need to be able to look at entry point at close ranges
/// </summary>
public class SetCamClipPlane : MonoBehaviour
{
    [SerializeField]
    private float _nearClipPlane;

    void Start()
    {
        Camera cam = gameObject.GetComponent<Camera>();
        if (cam != null)
            cam.nearClipPlane = _nearClipPlane;
    }
}
