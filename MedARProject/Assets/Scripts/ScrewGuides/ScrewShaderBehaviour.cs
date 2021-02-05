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
    private ScrewGuide guide;

    void Start()
    {
        mats = GetComponent<Renderer>().materials;
        guide = GetComponentInParent<ScrewGuide>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitResult;
        bool hit = false;
        hit = Physics.Raycast(guide.EntryPosition - guide.Direction * 0.1f, guide.Direction, out hitResult, 1f, LayerMask.GetMask("Spine"));

        if (hit == false)
        {
            for (int i = 0; i < mats.Length; i++)
                mats[i].SetFloat("_UseEntryPoint", 0);
        }
        else
        {
            float localPosY = transform.InverseTransformPoint(hitResult.point).y;
            
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].SetFloat("_UseEntryPoint", 1);
                mats[i].SetFloat("_YEntryPoint", localPosY);
            }
        }
    }
}
