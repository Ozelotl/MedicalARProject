﻿//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider that tells tool its current state
///     1) Collider that is inside tool - if it is touching the spine, the tool must be inside the spine
///     2) Collider that is on tooltip with radius - if 1) not touching, we are on the spine
/// </summary>
public class ColliderToolState : MonoBehaviour
{
    [SerializeField]
    private TrackedTool.ToolState _stateToSet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Spine"))
        {
            switch (_stateToSet)
            {
                case TrackedTool.ToolState.OnSpine:
                    TrackedTool.Instance.tooltipTouchingSpine = true;
                    Debug.LogWarning("Touching");
                    break;
                case TrackedTool.ToolState.InSpine:
                    TrackedTool.Instance.toolInsideSpine = true;
                    Debug.LogWarning("Inside");
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Spine"))
        {
            switch (_stateToSet)
            {
                case TrackedTool.ToolState.OnSpine:
                    TrackedTool.Instance.tooltipTouchingSpine = false;
                    Debug.LogWarning("NOT Touching");
                    break;
                case TrackedTool.ToolState.InSpine:
                    TrackedTool.Instance.toolInsideSpine = false;
                    Debug.LogWarning("NOT Inside");
                    break;
            }
        }
    }
}
