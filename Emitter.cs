using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamEmitter : MonoBehaviour
{
    [SerializeField] private Transform beamOrigin; // Where the beam starts
    [SerializeField] private float maxDistance = 30f; // Max distance for each beam segment
    [SerializeField] private int maxReflections = 10; // Max number of bounces
    [SerializeField] private LayerMask beamMask; // What layers the beam can hit

    private LineRenderer lineRenderer; // Draws the beam line

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>(); // Get the LineRenderer on this object
    }

    private void Update()
    {
        DrawBeam(); // Redraw beam every frame
    }

    private void DrawBeam()
    {
        List<Vector3> points = new List<Vector3>(); // Stores all beam points for the line
        HashSet<BeamReceiver> hitReceivers = new HashSet<BeamReceiver>(); // Stores receivers hit this frame

        Vector3 currentOrigin = beamOrigin.position; // Current start point of beam
        Vector3 currentDirection = beamOrigin.forward; // Current beam direction

        points.Add(currentOrigin); // Add starting point to line

        for (int i = 0; i < maxReflections; i++)
        {
            if (Physics.Raycast(currentOrigin, currentDirection, out RaycastHit hit, maxDistance, beamMask))
            {
                points.Add(hit.point); // Add hit point to beam line

                MirrorReflector mirror = hit.collider.GetComponentInParent<MirrorReflector>(); // Check if hit object is a mirror
                if (mirror != null)
                {
                    if (mirror.IsReflectiveCollider(hit.collider))
                    {
                        currentOrigin = mirror.GetBeamExitPosition(); // New beam start after reflection
                        currentDirection = mirror.GetBeamDirection(); // New beam direction after reflection

                        points.Add(currentOrigin); // Add new start point after bounce
                        continue; // Keep tracing reflected beam
                    }

                    break; // Stop if mirror side is not reflective
                }

                BeamReceiver receiver = hit.collider.GetComponentInParent<BeamReceiver>(); // Check if hit object is a receiver
                if (receiver != null)
                {
                    hitReceivers.Add(receiver); // Mark receiver as hit
                }

                break; // Stop beam after hitting non-mirror object
            }
            else
            {
                Vector3 endPoint = currentOrigin + currentDirection * maxDistance; // Beam goes full distance if nothing hit
                points.Add(endPoint); // Add final beam point
                break; // Stop tracing
            }
        }

        UpdateReceivers(hitReceivers); // Turn receivers on or off

        lineRenderer.positionCount = points.Count; // Set number of points in line
        lineRenderer.SetPositions(points.ToArray()); // Draw beam using saved points
    }

    private void UpdateReceivers(HashSet<BeamReceiver> hitReceivers)
    {
        BeamReceiver[] receivers = FindObjectsByType<BeamReceiver>(FindObjectsSortMode.None); // Find all receivers in scene

        foreach (BeamReceiver receiver in receivers)
        {
            receiver.SetPowered(hitReceivers.Contains(receiver)); // Power only receivers hit by beam
        }
    }
}