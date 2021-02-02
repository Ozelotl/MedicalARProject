//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets value of Spine hit pos for Screw Shader 
/// (to know location under which it should be transparent)
/// </summary>
public class ScrewShaderBehaviour : MonoBehaviour
{
    private Material[] mats;

    void Start()
    {
        mats = GetComponent<Renderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit? hit = TrackedTool.Instance.HitSpine;
        if (hit == null)
        {
            for (int i = 0; i < mats.Length; i++)
                mats[i].SetFloat("_UseEntryPoint", 0);
        }
        else
        {
            float localPosY = transform.InverseTransformPoint(TrackedTool.Instance.HitSpine.Value.point).y;
            
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].SetFloat("_UseEntryPoint", 1);
                mats[i].SetFloat("_YEntryPoint", localPosY);
            }
        }
    }
}
