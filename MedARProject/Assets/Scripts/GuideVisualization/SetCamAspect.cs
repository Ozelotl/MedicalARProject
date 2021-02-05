using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamAspect : MonoBehaviour
{
    [SerializeField]
    private float _aspect;

    void Start()
    {
        gameObject.GetComponent<Camera>().aspect = _aspect;
    }
}
