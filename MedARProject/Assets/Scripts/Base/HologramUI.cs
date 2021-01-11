//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;


/// <summary>
/// Behaviour to "billboard" ui in front of hologram (copied from AppBar)
/// </summary>
public class HologramUI : MonoBehaviour
{
    private const float moveSpeed = 5;

    [SerializeField]
    private BoxCollider _collider;

    [SerializeField]
    private float _offset = 0f;

    private BoundingBoxHelper helper = new BoundingBoxHelper();
    private List<Vector3> boundsPoints = new List<Vector3>();

    private void OnEnable()
    {
        FollowTargetObject(false);
    }

    private void Update()
    {
        FollowTargetObject(true);
    }

    private void FollowTargetObject(bool smooth)
    {
        if (_collider == null)
            return;

        // Calculate the best follow position
        Vector3 finalPosition = Vector3.zero;
        Vector3 headPosition = CameraCache.Main.transform.position;
        boundsPoints.Clear();

        helper.UpdateNonAABoundsCornerPositions(_collider, boundsPoints);
        int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
        Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

        // Finalize the new position
        finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * _offset);

        // Follow our bounding box
        transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * moveSpeed) : finalPosition;

        // Rotate on the y axis
        Vector3 direction = (_collider.bounds.center - finalPosition).normalized;
        if (direction != Vector3.zero)
        {
            Vector3 eulerAngles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

}
