using UnityEngine;

public class MirrorReflector : MonoBehaviour
{
    [SerializeField] private Transform beamExitPoint; // Where reflected beam starts from
    [SerializeField] private Transform beamExitDirection; // Direction reflected beam goes
    [SerializeField] private Collider reflectiveCollider; // Collider that is allowed to reflect beam

    public bool IsReflectiveCollider(Collider hitCollider)
    {
        return hitCollider == reflectiveCollider; // True only if beam hit reflective side
    }

    public Vector3 GetBeamExitPosition()
    {
        if (beamExitPoint != null)
            return beamExitPoint.position; // Use custom beam exit point

        return transform.position; // Fallback to mirror position
    }

    public Vector3 GetBeamDirection()
    {
        if (beamExitDirection != null)
            return beamExitDirection.forward.normalized; // Use custom reflected direction

        return transform.forward.normalized; // Fallback to object forward direction
    }
}