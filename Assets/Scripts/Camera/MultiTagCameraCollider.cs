using UnityEngine;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;

[DisallowMultipleComponent]
[ExecuteAlways]
[AddComponentMenu("")] // Ukryte w menu
public class MultiTagCameraCollider : CinemachineExtension
{
    [Header("Obstacle Detection")]
    [Tooltip("Objects on these layers will be detected")]
    public LayerMask CollideAgainst = 1;

    [Tooltip("Objects on these layers will never obstruct view of the target")]
    public LayerMask TransparentLayers = 0;

    [Tooltip("Minimum distance from target to ignore obstacles")]
    public float MinimumDistanceFromTarget = 0.1f;

    [Tooltip("Camera radius for collision handling")]
    public float CameraRadius = 0.1f;

    [Tooltip("Tags of objects to ignore collisions with (e.g., Player, Sword)")]
    public List<string> IgnoreTags = new List<string>() { "Player", "Sword" };

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body || !vcam.Follow)
            return;

        Vector3 camPos = state.CorrectedPosition;
        Vector3 lookAtPos = state.HasLookAt ? state.ReferenceLookAt : camPos + vcam.transform.forward;

        Vector3 dir = camPos - lookAtPos;
        float distance = dir.magnitude;

        if (distance > Mathf.Epsilon)
        {
            dir /= distance;

            if (Physics.SphereCast(lookAtPos, CameraRadius, dir, out RaycastHit hit, distance,
                CollideAgainst & ~TransparentLayers, QueryTriggerInteraction.Ignore))
            {
                if (!IgnoreTags.Contains(hit.collider.tag))
                {
                    state.PositionCorrection += dir * (hit.distance - distance);
                }
            }
        }
    }
}
