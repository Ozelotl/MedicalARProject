//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Aspect ratio of orthogonal camera can't be set in the Editor - expose this
//Needed for 2D view additional cameras
public class SetCamAspect : MonoBehaviour
{
    [SerializeField]
    private float _aspect;

    void Start()
    {
        gameObject.GetComponent<Camera>().aspect = _aspect;
    }
}
